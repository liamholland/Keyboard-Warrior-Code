using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask whatIsEnemy;
    public GameObject keyboard; //reference to the keyboard
    public CameraController cameraController;
    public Controller playerController; //reference to the player controller
    [SerializeField] private int health;
    [SerializeField] private float attackRange;
    [SerializeField] private float keyboardThrowForce;  //speed at which the keyboard moves towards the target
    [Range(1f, 30f)] [SerializeField] private float keyboardThrowRange;
    [Range(0f, 0.1f)] [SerializeField] private float attackShakeTime;
    [SerializeField] private float attackShakeSpeed;
    [SerializeField] private LayerMask whatIsGrapplePoint;  //the layer that the keyboard can grapple to

    private bool keyboardThrown = false;    //has the keyboard been thrown
    private Vector2 keyboardTarget; //the target for the keyboard to move towards
    private Vector2 keyboardStartPosition;  //the position the keyboard is at relative to the player object

    private void Update()
    {
        //when the player attacks
        if (Input.GetButtonDown("Attack") && !keyboardThrown)
        {
            Attack();
        }
        //when the player performs a ranged attack / grapple
        else if(keyboard.activeSelf && Input.GetButtonDown("Throw") && !keyboardThrown){
            StartCoroutine(ThrowKeyBoard());
        }
        
        keyboardStartPosition = transform.position;

        if(keyboardThrown){
            keyboard.transform.position = Vector2.MoveTowards(keyboard.transform.position, keyboardTarget, keyboardThrowForce * Time.deltaTime);
            
            //if the keyboard has returned stop trying to return it
            if(Vector2.Distance(keyboardStartPosition, keyboard.transform.position) < 0.05f){
                keyboardThrown = false;
            }
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
                Enemy i = collider.gameObject.GetComponent<Enemy>();
                i.TakeDamage(1, 1);
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
            //keyboard has hit the ground - stop the coroutine
            StopCoroutine(ThrowKeyBoard());
            
            keyboardTarget = keyboardStartPosition; //reset keyboard target
        }
    }

    //coroutine for throwing the keyboard
    private IEnumerator ThrowKeyBoard(){
        keyboardTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //get the mouse position

        keyboardThrown = true;  //the keyboard is thrown

        yield return new WaitUntil(() => Vector2.Distance(keyboard.transform.position, keyboardTarget) < 0.2f ||
            Vector2.Distance(transform.position, keyboard.transform.position) > keyboardThrowRange);

        Collider2D grapplePoint = Physics2D.OverlapCircle(keyboard.transform.position, 0.2f, whatIsGrapplePoint);

        if(grapplePoint == null){
            keyboardTarget = keyboardStartPosition;
        }
        else{
            keyboardTarget = grapplePoint.gameObject.transform.position;    //move the keyboard to the actual point
            
            //start the coroutine where the player will grapple to the point
            StartCoroutine(playerController.grappleToPoint(grapplePoint.gameObject.transform.position));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, keyboardThrowRange);
    }
}
