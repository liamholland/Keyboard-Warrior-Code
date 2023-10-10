using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask whatIsEnemy;
    public GameObject keyboard; //reference to the keyboard
    public Controller playerController; //reference to the player controller
    [SerializeField] private int health;
    [SerializeField] private float attackRange;
    [SerializeField] private float keyboardThrowForce;  //speed at which the keyboard moves towards the target
    [Range(1f, 30f)] [SerializeField] private float keyboardGrappleRange;
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowAttackRange;
    [Range(0f, 0.1f)] [SerializeField] private float attackShakeTime;
    [SerializeField] private float attackShakeSpeed;
    [SerializeField] private LayerMask whatIsGrapplePoint;  //the layer that the keyboard can grapple to

    private CameraController cameraController;
    private bool keyboardThrown = false;    //has the keyboard been thrown
    private bool keyboardHooked = false;    //has the keyboard hooked on to a grapple point
    private Vector2 keyboardTarget; //the target for the keyboard to move towards
    private Vector2 keyboardStartPosition;  //the position the keyboard is at relative to the player object

    private void Awake(){
        //get a reference to the camera controller
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        //keyboard actions
        if (Input.GetButtonDown("Attack") && !keyboardThrown)
        {
            Attack();
        }
        //when the player performs a ranged attack / grapple
        else if(keyboard.activeSelf && Input.GetButtonDown("ThrowGrapple") && !keyboardThrown){
            StartCoroutine(ThrowKeyBoard());
        }
        else if(keyboard.activeSelf && Input.GetButtonDown("ThrowAttack") && !keyboardThrown){
            StartCoroutine(RangeAttackWithKeyboard());
        }
        
        //for now, the start position is just the player's position
        keyboardStartPosition = transform.position;

        if(keyboardThrown){
            keyboard.transform.position = Vector2.MoveTowards(keyboard.transform.position, keyboardTarget, keyboardThrowForce * Time.deltaTime);
            
            //check if the keyboard has reached the target
            if(Vector2.Distance(keyboardTarget, keyboard.transform.position) < 0.05f){
                
                //if the keyboard has returned stop trying to return it
                if(Vector2.Distance(keyboardTarget, keyboardStartPosition) < 0.1f){
                    keyboardThrown = false; //keyboard is no longer thrown
                    keyboardHooked = false; //the keyboard is unhooked because the player has reached it
                    keyboard.transform.parent = transform;  //keyboard's position is affected by the player
                }
                else if(!keyboardHooked){
                    keyboardTarget = keyboardStartPosition; //otherwise set the target to the start position
                }
            }
        }

        //flip the player sprite to face the correct direction
        if(Input.GetAxisRaw("Horizontal") != 0f){
            transform.localScale = new Vector2(Input.GetAxisRaw("Horizontal"), transform.localScale.y); //flip the player
        }
    }

    private void Attack()
    {
        if(keyboard.activeSelf){
            //get an enemy collider
            Collider2D collider = Physics2D.OverlapCircle(keyboard.transform.position, attackRange, whatIsEnemy);
            
            //if there is a collider, do damage to it
            if(collider != null){
                StartCoroutine(cameraController.ShakeCamera(attackShakeSpeed, attackShakeTime, new Vector2(0f, 0.2f), new Vector2(transform.localScale.x * 0.8f, 0f)));
                Enemy e = collider.gameObject.GetComponent<Enemy>();
                e.TakeDamage(1, 1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        //if the player loses all its health, kill the player
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")){
            //keyboard has hit the ground - stop coroutines
            StopAllCoroutines();
            
            keyboardTarget = keyboardStartPosition; //reset keyboard target
        }
    }

    //coroutine for a long range attack with a keyboard
    private IEnumerator RangeAttackWithKeyboard(){
        keyboard.transform.parent = null;   //stop the position of the keyboard being affected by the player

        keyboardTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        keyboardThrown = true;  //the keyboard is thrown

        yield return new WaitUntil(() => Vector2.Distance(keyboard.transform.position, keyboardTarget) < 0.2f ||
            Vector2.Distance(transform.position, keyboard.transform.position) > keyboardThrowAttackRange);

        Attack();   //attack when it reaches its destination

        keyboardTarget = keyboardStartPosition;
    }

    //coroutine for throwing the keyboard as a grapple hook
    private IEnumerator ThrowKeyBoard(){
        keyboard.transform.parent = null;   //stop the position of the keyboard being affected by the player
        
        keyboardTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        keyboardThrown = true;  //the keyboard is thrown

        yield return new WaitUntil(() => Vector2.Distance(keyboard.transform.position, keyboardTarget) < 0.2f ||
            Vector2.Distance(transform.position, keyboard.transform.position) > keyboardGrappleRange);

        Collider2D grapplePoint = Physics2D.OverlapCircle(keyboard.transform.position, 0.2f, whatIsGrapplePoint);

        if(grapplePoint == null){
            keyboardTarget = keyboardStartPosition;
        }
        else{
            keyboardTarget = grapplePoint.gameObject.transform.position;    //move the keyboard to the actual point
            
            keyboardHooked = true;  //the keyboard is hooked on to a grapple point

            //start the coroutine where the player will grapple to the point
            StartCoroutine(playerController.grappleToPoint(grapplePoint.gameObject.transform.position));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, keyboardGrappleRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, keyboardThrowAttackRange);
    }
}
