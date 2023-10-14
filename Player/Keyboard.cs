using System;
using System.Collections;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    public GameObject player;   //reference to the player

    [Header("-- Unlocks --")]
    [SerializeField] private bool available = false;   //has the keyboard been unlocked at all
    public bool longCableUnlocked = false;  //the longer cable allows you to throw the keyboard to use it as a grapple and a long range weapon
    
    //public accessor for available
    [HideInInspector]
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
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowForce;  //speed at which the keyboard moves towards the target
    [Range(1f, 30f)] [SerializeField] private float keyboardGrappleRange;
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowAttackRange;
    
    [HideInInspector] public Vector2 startPosition;  //the position the keyboard started from
    [HideInInspector] public bool IsHooked { get => hooked; } //is the keyboard hooked - read only
    [HideInInspector] public bool IsThrown { get => thrown; }   //is the keyboard thrown - read only
    
    private Controller playerController;    //reference to the player controller
    private SpriteRenderer keyboardRenderer;    //reference to the keyboard's renderer
    private BoxCollider2D keyboardCollider; //reference to the keyboard's collider
    private Vector2 target; //the target position the keyboard is heading for
    private bool thrown = false;    //has the keyboard been thrown
    private bool hooked = false;    //has the keyboard been hooked to a grapple point

    [Header("-- Coding Level --")]
    public int level = 0;    //the level of the player

    private void Awake(){
        //get a reference to the player controller
        playerController = player.GetComponent<Controller>();

        //get the renderer reference
        keyboardRenderer = gameObject.GetComponent<SpriteRenderer>();
        keyboardRenderer.enabled = available;

        //get a reference to the collider
        keyboardCollider = gameObject.GetComponent<BoxCollider2D>();
        keyboardCollider.enabled = available;

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
                }
                else if(!hooked){
                    target = startPosition; //otherwise set the target to the start position
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        //if the collision is with the ground
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //keyboard has hit the ground - stop coroutines
            StopAllCoroutines();

            target = startPosition; //reset keyboard target
        }
    }

    //coroutine for throwing the keyboard as a grapple hook
    public IEnumerator ThrowKeyBoard(){
        transform.parent = null;   //stop the position of the keyboard being affected by the player

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
            StartCoroutine(playerController.grappleToPoint(grapplePoint.gameObject.transform.position));
        }
    }

    //coroutine for a long range attack with a keyboard
    public IEnumerator RangeAttackWithKeyboard(){
        transform.parent = null;   //stop the position of the keyboard being affected by the player

        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        thrown = true;  //the keyboard is thrown

        yield return new WaitUntil(() => Vector2.Distance(transform.position, target) < 0.2f ||
            Vector2.Distance(player.transform.position, transform.position) > keyboardThrowAttackRange);

        playerController.Attack();   //attack when it reaches its destination

        target = startPosition;
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, keyboardGrappleRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(player.transform.position, keyboardThrowAttackRange);
    }
}
