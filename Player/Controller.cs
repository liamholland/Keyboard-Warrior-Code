using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("-- Movement --")]
    public bool airControl; //can the player move in the air
    public bool canDash;    //can the player dash
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHoldDuration;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float GCRadius;
    [SerializeField] private LayerMask whatIsGround;
    [Range(20f, 50f)] [SerializeField] private float grappleSpeed;  //the speed at which the player grapples towards a point


    [Header("-- Dash --")]
    [Range(25f, 50f)] [SerializeField] private float dashSpeed;
    [Range(0f, 10f)] [SerializeField] private float dashCoolDown;
    [Range(0f, 0.5f)] [SerializeField] private float dashTime;  //the amount of time the dash lasts for
    [Range(0f, 0.5f)] [SerializeField] private float verticalDashLimit; //limit applied to vertical dash so they don't feel way longer than a normal dash
    [Range(0f, 0.5f)] [SerializeField] private float dashHangTime;    //the amount of time you hang in the air after a dash
    [Range(0f, 3f)] [SerializeField] private float dashXShakeMagnitude;  //the magnitude of the dash on the x axis
    [Range(0f, 0.1f)] [SerializeField] private float dashShakeTime;    //the longest time the camera can shake for at once
    [Range(0f, 0.1f)] [SerializeField] private float attackShakeTime;
    [SerializeField] private float dashShakeSpeed;  //the speed of the camera when shaking during a dash
    [SerializeField] private Color dashColor;


    [Header("-- Combat --")]
    public LayerMask whatIsEnemy;
    [SerializeField] private float attackRange;
    [SerializeField] private int health;
    [SerializeField] private float attackShakeSpeed;


    [Header("-- Other --")]
    public KeyboardController keyboardController; //reference to the keyboard controller
    [SerializeField] private Animator dialogueBoxAnimator;  //the animator for the dialogueBox
    [SerializeField] private float deathZoneY;  //the y below which the player is dead
    public static bool interacting = false; //is the player interacting with something

    private SpriteRenderer playerRenderer;
    private Rigidbody2D playerRigid;
    private CameraController cameraController;  //reference to the camera controller
    private bool jumpAvailable = true;
    private bool dashAvailable = true;
    private bool isDashing = false;
    private float lastGroundedAt = -1f; //essentially a timer for how long the player has been in the air
    private bool atGrapplePoint = false;    //is the player hooked to a grapple point
    private bool isGrappling = false;   //is the player currently grappling
    private float playerGravity;    //player gravity used to save the player's gravity when needs to be changed to 0 temporarily
    private Vector2 lastGroundedPosition;   //the last place the player was on the ground

    private void Awake(){
        //get a reference to the camera controller
        cameraController = Camera.main.GetComponent<CameraController>();

        //get a reference to the renderer
        playerRenderer = GetComponent<SpriteRenderer>();

        //get a reference to the rigid body
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void Update(){
        //make jump available when the key is released
        if(Input.GetButtonUp("Jump")){
            jumpAvailable = true;
        }

        //flip the player sprite to face the correct direction
        if(Input.GetAxisRaw("Horizontal") != 0f){
            transform.localScale = new Vector2(Input.GetAxisRaw("Horizontal"), transform.localScale.y); //flip the player
        }

        //keyboard actions

        //if the player is attacking, the keyboard is not thrown and the keyboard is unlocked
        if (Input.GetButtonDown("Attack") && !keyboardController.IsThrown && keyboardController.KeyboardAvailable){
            Attack();
        }
        //when the player performs a ranged attack / grapple
        else if(Input.GetButtonDown("ThrowGrapple") && !keyboardController.IsThrown && keyboardController.longCableUnlocked){
            StartCoroutine(keyboardController.ThrowKeyBoard());
        }
        else if(Input.GetButtonDown("ThrowAttack") && !keyboardController.IsThrown && keyboardController.longCableUnlocked){
            StartCoroutine(keyboardController.RangeAttackWithKeyboard());
        }

        //for now, the start position is just the player's position
        keyboardController.startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if(interacting) return;   //the player cannot move if they are interacting with something

        //if the player fell off the map
        if(transform.position.y < deathZoneY){
            transform.position = lastGroundedPosition;  //put the player back to where they were
            playerRigid.velocity = Vector2.zero;    //remove any velocity the player has
        }
        else {
            Move();
        }
    }

    private void Move() 
    {
        if(isDashing || isGrappling){
            return;
        }

        bool grounded = CheckIfGrounded();

        if(grounded) lastGroundedPosition = transform.position; //save the place where the player was last grounded

        if ((grounded || airControl) && !atGrapplePoint){
            playerRigid.velocity = new Vector2(maxMoveSpeed * Input.GetAxisRaw("Horizontal"), playerRigid.velocity.y);   //set the velocity of the player
        }

        //set the last grounded time if the player is allowed to jump
        if((grounded || atGrapplePoint) && jumpAvailable){
            lastGroundedAt = Time.time;
        }

        //if the player tries to jump and they have some "jump allowance"
        if(Input.GetButton("Jump") && Time.time < lastGroundedAt + jumpHoldDuration){
            //apply some force to the player's vertical velocity
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, jumpForce);
            
            //stop the player from jumping again if they are holding the button and on the ground
            if(grounded){
                jumpAvailable = false;
            }
        }
        else if(canDash && Input.GetButton("Dash") && dashAvailable){
            StartCoroutine(Dash());
        }
    }

    //what the player does when they attack
    public void Attack()
    {
        //get an enemy collider
        Collider2D collider = Physics2D.OverlapCircle(keyboardController.gameObject.transform.position, attackRange, whatIsEnemy);
        
        //if there is a collider, do damage to it
        if(collider != null){
            StartCoroutine(cameraController.ShakeCamera(attackShakeSpeed, attackShakeTime, new Vector2(0f, 0.2f), new Vector2(transform.localScale.x * 0.8f, 0f)));
            Enemy e = collider.gameObject.GetComponent<Enemy>();
            e.TakeDamage(1, 1);
        }
    }

    //what to do when the player takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        //if the player loses all its health, kill the player
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    //Dash Ability
    private IEnumerator Dash(){
        //the player is now dashing
        dashAvailable = false;
        isDashing = true;

        //change the player's gravity to 0 if not at a grapple point
        if(!atGrapplePoint){
            playerGravity = playerRigid.gravityScale;
            playerRigid.gravityScale = 0f;
        }

        //change the color of the player's sprite
        Color playerColour = playerRenderer.color;
        playerRenderer.color = dashColor;
        
        //apply the dash
        playerRigid.velocity = new Vector2(dashSpeed * transform.localScale.x, transform.position.y);

        //shake the camera - applied only in the direction opposite to the direction of the dash
        StartCoroutine(cameraController.ShakeCamera(dashShakeSpeed, dashShakeTime, new Vector2(transform.localScale.x * dashXShakeMagnitude, Input.GetAxisRaw("Vertical")) * 0.4f, Vector2.zero));

        //dash in progress
        yield return new WaitForSeconds(Input.GetAxisRaw("Vertical") > 0f ? dashTime - verticalDashLimit : dashTime);

        //hang in the air for a moment
        playerRigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashHangTime);

        //return gravity to normal
        playerRigid.gravityScale = playerGravity;
        
        //the player is no longer dashing
        isDashing = false;
        
        //cooldown
        yield return new WaitForSeconds(dashCoolDown);

        //reset the color of the sprite
        playerRenderer.color = playerColour;

        dashAvailable = true;
    }

    //grapple ability
    public IEnumerator grappleToPoint(Vector2 grapplePoint){        
        //wait to finish the dash coroutine before starting the grapple one
        if(isDashing) yield return new WaitUntil(() => !isDashing);
        
        isGrappling = true; //the player is now grappling

        //save the gravity scale of the player
        float playerGravity = playerRigid.gravityScale;

        //remove gravity on the player
        playerRigid.gravityScale = 0f;

        //grapple to the point
        playerRigid.velocity = new Vector2(grapplePoint.x - transform.position.x, grapplePoint.y - transform.position.y).normalized * grappleSpeed;

        //wait until the player reaches the grapple point
        yield return new WaitUntil(() => Vector2.Distance(transform.position, grapplePoint) < 0.1f);

        //set the velocity to 0
        playerRigid.velocity = Vector2.zero;

        isGrappling = false;    //player no longer grappling

        atGrapplePoint = true;  //player is at a grapple point

        //wait until the player is moving away from the grapple point
        yield return new WaitUntil(() => Vector2.Distance(transform.position, grapplePoint) > 1f);

        //return gravity to the player
        playerRigid.gravityScale = playerGravity;

        atGrapplePoint = false; //player is no longer at the grapple point
    }

    private bool CheckIfGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, GCRadius, whatIsGround);
        return colliders.Length > 0;
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(0f, deathZoneY), 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.transform.position, GCRadius);
    }
}
