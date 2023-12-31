using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    public GameObject player;   //reference to the player

    [Header("-- Unlocks --")]
    [SerializeField] private bool available = false;   //has the keyboard been unlocked at all
    public bool longCableUnlocked = false;  //the longer cable allows you to throw the keyboard to use it as a grapple and a long range weapon
    public List<Key> keys = new List<Key>();  //the keys the player has collected for the keyboard
    
    //public accessor for available
    public bool KeyboardAvailable{
        get => available;
        set {
            available = value;
            //enable or disable the keyboard
            if(value == false){
                keyboardRenderer.enabled = false;
                keyboardCollider.enabled = false;
            }
            else if(value == true){
                keyboardRenderer.enabled = true;
                keyboardCollider.enabled = true;
            }
        }
    }

    [Header("-- Throwing --")]
    public LayerMask whatIsGrapplePoint;    //reference to the grapple point layer
    public LayerMask whatIsObstacle;    //what is an obstacle to the thrown keyboard
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowForce;  //speed at which the keyboard moves towards the target
    [Range(1f, 30f)] [SerializeField] private float keyboardGrappleRange;
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowAttackRange;
    [SerializeField] private Attack throwAttack;    //the attack of performed when thrown
    [SerializeField] private LineRenderer cable;    //the rendered cable
    public Vector2 startPosition { get; set; }  //the position the keyboard started from
    public bool IsHooked { get => hooked; } //is the keyboard hooked - read only
    public bool IsThrown { get => thrown; }   //is the keyboard thrown - read only
    
    [Header("-- Coding Level --")]
    [SerializeField] private int level = 0;    //the level of the player
    [SerializeField] private Notification levelUpNotification;  //notification displayed when the player levels up

    private Controller playerController;    //reference to the player controller
    private SpriteRenderer keyboardRenderer;    //reference to the keyboard's renderer
    private BoxCollider2D keyboardCollider; //reference to the keyboard's collider
    private Vector2 target; //the target position the keyboard is heading for
    private Animator playerAnimator;    //reference to the player animator for rebinding animations
    private bool thrown = false;    //has the keyboard been thrown
    private bool hooked = false;    //has the keyboard been hooked to a grapple point

    //public accessor for level
    public int Level {
        get => level;
        set{
            string newLevel = FormatLevel(value);
            currentLevelUI.text = newLevel;   //update the level ui

            //update the player context (save the game)
            Controller.context = PlayerContext.GenerateNewContext(playerController, this);

            //show notification
            levelUpNotification.notificationText = "New Level: " + newLevel;

            if(NotificationManager.Manager != null && level != value){
                NotificationManager.Manager.ShowPopUpNotification(levelUpNotification);
            }

            level = value;  //set the value
        }
    }
    [SerializeField] private TextMeshProUGUI currentLevelUI;    //reference to the player UI that displays the level

    private void Awake(){
        //get a reference to the player controller
        playerController = player.GetComponent<Controller>();

        //get the renderer reference
        keyboardRenderer = gameObject.GetComponent<SpriteRenderer>();
        keyboardRenderer.enabled = available;

        //get a reference to the collider
        keyboardCollider = gameObject.GetComponent<BoxCollider2D>();
        keyboardCollider.enabled = available;

        //get a reference to the player animator
        playerAnimator = player.GetComponent<Animator>();

        KeyboardAvailable = available;  //can set the availability of the keyboard based on the inspector value at the start of the game
    }

    private void Update(){
        if(thrown){
            transform.position = Vector2.MoveTowards(transform.position, target, keyboardThrowForce * Time.deltaTime);
            
            //check if the keyboard has reached the target
            if(Vector2.Distance(target, transform.position) < 0.05f){
                //if the keyboard has returned stop trying to return it
                if(Vector2.Distance(target, startPosition) < 0.1f){
                    thrown = false; //keyboard is no longer thrown
                    hooked = false; //the keyboard is unhooked because the player has reached it
                    transform.parent = player.transform;  //keyboard's position is affected by the player
                    playerAnimator.Rebind();   //animations will stop working without this call
                }
                else if(!hooked){
                    target = startPosition; //otherwise set the target to the start position
                }
            }

            //render the cable
            cable.enabled = true;
            cable.SetPosition(0, transform.position);
            cable.SetPosition(1, player.transform.position);
        }
        else{
            cable.enabled = false;
        }
    }

    /// <summary>
    /// Returns the level formatted as a string with relative C language
    /// Example:
    /// level=3 -> "C#"
    /// level=4 -> "C#++"
    /// </summary>
    /// <param name="level">The level as an integer</param>
    /// <returns>A string representation of the level</returns>
    public string FormatLevel(int level){
        if(level < 1) return " ";    //there is no format for levels less than 1
        
        string res = "C";   //level at 1 or greater will always start with 'C'

        //if the level has reached at least 3
        if(level >= 3){
            res += "#"; //it is at the level of C#
            res += Mathf.Floor((level - 3) / 2) + 1; //modifier for the sharp level
        }

        //if the level is odd, it needs a ++ next to it
        if(level >= 2 && level % 2 == 0){
            res += "++";
        }

        return res; //return the result
    }

    void OnTriggerEnter2D(Collider2D other){
        //if the keyboard is thrown and the layer of the collider is an obstacle
        if (IsThrown && (((1 << other.gameObject.layer) & whatIsObstacle) != 0))
        {
            //keyboard has hit the ground - stop coroutines
            StopAllCoroutines();

            target = startPosition; //reset keyboard target
        }
    }

    //coroutine for throwing the keyboard as a grapple hook
    public IEnumerator ThrowKeyBoard(){
        Vector2 currPos = transform.position;
        
        transform.parent = null;   //stop the position of the keyboard being affected by the player

        playerAnimator.Rebind();   //prevent the keyboard from being affected by animations

        transform.position = currPos;

        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        thrown = true;

        yield return new WaitUntil(() => Vector2.Distance(transform.position, target) < 0.2f ||
            Vector2.Distance(player.transform.position, transform.position) > keyboardGrappleRange);

        Collider2D grapplePoint = Physics2D.OverlapCircle(transform.position, 0.2f, whatIsGrapplePoint);

        if(grapplePoint == null){
            target = startPosition;
        }
        else{
            target = grapplePoint.gameObject.transform.position;    //move the keyboard to the actual point
            
            hooked = true;  //the keyboard is hooked on to a grapple point

            //start the coroutine where the player will grapple to the point
            StartCoroutine(playerController.GrappleToPoint(grapplePoint.gameObject.transform.position));
        }
    }

    //coroutine for a long range attack with a keyboard
    public IEnumerator RangeAttackWithKeyboard(){
        Vector2 currPos = transform.position;
        
        transform.parent = null;   //stop the position of the keyboard being affected by the player

        playerAnimator.Rebind();   //prevent the keyboard from being affected by animations

        transform.position = currPos;

        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        thrown = true;  //the keyboard is thrown

        yield return new WaitUntil(() => Vector2.Distance(transform.position, target) < 0.2f ||
            Vector2.Distance(player.transform.position, transform.position) > keyboardThrowAttackRange);

        playerController.Attack(throwAttack);   //attack when it reaches its destination

        target = startPosition;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, keyboardGrappleRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(player.transform.position, keyboardThrowAttackRange);
    }
}
