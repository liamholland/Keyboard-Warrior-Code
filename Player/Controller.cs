using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [Header("-- Movement --")]
    public bool airControl; //can the player move in the air
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHoldDuration;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float GCRadius;
    [SerializeField] private LayerMask whatIsGround;
    [Range(20f, 50f)] [SerializeField] private float grappleSpeed;  //the speed at which the player grapples towards a point
    [Range(0f, 1f)] [SerializeField] private float momentumMaintainingScaler;   //the percentage of momentum maintained when maintaining momentum in the air


    [Header("-- Dash --")]
    public bool canDash;    //can the player dash
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
    public LayerMask whatIsEnemy;   //reference to the enemy physics layer
    
    //the player's basic attacks
    [SerializeField] private Attack sideAttack;
    [SerializeField] private Attack upAttack;
    [SerializeField] private Attack downAttack;
    [SerializeField] private float attackShakeSpeed;


    [Header("-- Other --")]
    public KeyboardController keyboardController; //reference to the keyboard controller
    [SerializeField] private Animator playerAnimator;   //reference to the player animator
    [SerializeField] private Animator sceneTransitionAnimator;  //reference to the animator on the scene transition
    [SerializeField] private float deathZoneY;  //the y below which the player is dead
    public static bool isInteracting = false; //is the player interacting with something

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
    private bool maintainMomentum = false;
    private Color playerColour;

    public static PlayerContext context;    //the context to load the player with

    private void Awake(){
        //get a reference to the camera controller
        cameraController = Camera.main.GetComponent<CameraController>();

        //get a reference to the renderer
        playerRenderer = GetComponent<SpriteRenderer>();

        //get a reference to the rigid body
        playerRigid = GetComponent<Rigidbody2D>();
    }

    private void Start(){
        //set the player colour
        playerColour = playerRenderer.color;

        isInteracting = false;

        //set the player context if there is one
        if(context != null){
            //set player values
            airControl = context.airControl;
            canDash = context.canDash;
            keyboardController.KeyboardAvailable = context.available;
            keyboardController.longCableUnlocked = context.longCableUnlocked;
            keyboardController.Level = context.level;
            keyboardController.keys = context.keys;

            //set player position
            transform.position = PlayerContext.spawnPosition;

            PlayerContext.ApplyContextToObjects();
        }
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

        //if the player is attacking, the keyboard is not thrown and the keyboard is unlocked and the player is not currently in an attack animation
        if (Input.GetButtonDown("Attack") && 
            !keyboardController.IsThrown && 
            keyboardController.KeyboardAvailable &&
            !playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            
            Attack attack;  //reference to the attack to use

            //check the attack to use
            if(Input.GetAxisRaw("Vertical") > 0){
                attack = upAttack;
            }
            else if(Input.GetAxisRaw("Vertical") < 0){
                attack = downAttack;
            }
            else{   //defaults to a side attack
                attack = sideAttack;
            }
            
            Attack(attack);
        }
        //when the player performs a ranged attack / grapple
        else if(Input.GetButtonDown("ThrowGrapple") && !keyboardController.IsThrown && keyboardController.longCableUnlocked){
            StartCoroutine(keyboardController.ThrowKeyBoard());
        }
        else if(Input.GetButtonDown("ThrowAttack") && !keyboardController.IsThrown && keyboardController.longCableUnlocked){
            StartCoroutine(keyboardController.RangeAttackWithKeyboard());
        }
        else if(Input.GetKeyDown(KeyCode.Escape)){
            StartCoroutine(LoadSceneAnimation("MainMenu"));
        }

        //for now, the start position is just the player's position
        keyboardController.startPosition = transform.position;
    }

    void FixedUpdate()
    {
        //if the player fell off the map
        if(transform.position.y < deathZoneY){
            transform.position = lastGroundedPosition;  //put the player back to where they were
            playerRigid.velocity = Vector2.zero;    //remove any velocity the player has
        }
        else {
            Move();
        }
    }

    //move the player character
    private void Move() 
    {
        //dont move if the player is grappling or dashing or interacting
        if(isDashing || isGrappling || isInteracting){
            return;
        }

        bool grounded = CheckIfGrounded();  //check if the player is grounded

        //if the player is grounded
        if(grounded){
            lastGroundedPosition = transform.position; //save the place where the player was last grounded
            maintainMomentum = false;   //stop maintaining momentum
        }

        float momentum = Input.GetAxisRaw("Horizontal");    //get the x-axis momentum of the player

        //if the player is in the air and moving
        if(!grounded && momentum != 0){
            maintainMomentum = true;    //start maintaining momentum
        }

        //if the player is in the air and not moving but was moving
        if(!grounded && momentum == 0 && maintainMomentum){
            momentum = momentumMaintainingScaler * transform.localScale.x;   //maintain some momentum
        }

        //move the player if they are grounded or are allowed to move in the air, and they are not at a grapple point
        if ((grounded || airControl) && !atGrapplePoint){
            playerRigid.velocity = new Vector2(maxMoveSpeed * momentum, playerRigid.velocity.y);   //set the velocity of the player
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

    /// <summary>
    /// Make the player attack
    /// </summary>
    /// <param name="attack">The attack that will be used</param>
    public void Attack(Attack attack)
    {
        //get an enemy collider
        Collider2D collider = Physics2D.OverlapCircle((Vector2)keyboardController.gameObject.transform.position + new Vector2(transform.localScale.x, Input.GetAxisRaw("Vertical") * 0.8f), attack.AttackDamageRange, whatIsEnemy);
        
        //if there is a collider, do damage to it
        if(collider != null){
            //shake the camera
            StartCoroutine(cameraController.ShakeCamera(attackShakeSpeed, attackShakeTime, new Vector2(0f, 0.2f), new Vector2(transform.localScale.x * 0.8f, 0f)));
        }
        
        attack.DoAttack(collider);  //do the attack
    }

    //Dash Ability
    private IEnumerator Dash(){
        //the player is now dashing
        maintainMomentum = false;
        dashAvailable = false;
        isDashing = true;

        //change the player's gravity to 0 if not at a grapple point
        if(!atGrapplePoint){
            playerGravity = playerRigid.gravityScale;
            playerRigid.gravityScale = 0f;
        }

        //change the color of the player's sprite
        playerRenderer.color = dashColor;
        
        //apply the dash
        playerRigid.velocity = new Vector2(dashSpeed * transform.localScale.x, 0f);

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
    /// <summary>
    /// Grapples the player to a point
    /// </summary>
    /// <param name="grapplePoint">The grapple point position</param>
    /// <returns></returns>
    public IEnumerator GrappleToPoint(Vector2 grapplePoint){        
        //wait to finish the dash coroutine before starting the grapple one
        if(isDashing) yield return new WaitUntil(() => !isDashing);
        
        isGrappling = true; //the player is now grappling

        if(playerRigid.gravityScale > 0f){
            //save the gravity scale of the player
            playerGravity = playerRigid.gravityScale;
        }

        //remove gravity on the player
        playerRigid.gravityScale = 0f;

        //grapple to the point
        playerRigid.velocity = (grapplePoint - (Vector2)transform.position).normalized * grappleSpeed;

        //wait until the player reaches the grapple point
        yield return new WaitUntil(() => Vector2.Distance(transform.position, grapplePoint) < 0.4f);

        //set the velocity to 0
        playerRigid.velocity = Vector2.zero;

        //set the player position
        transform.position = grapplePoint;

        isGrappling = false;    //player no longer grappling

        atGrapplePoint = true;  //player is at a grapple point

        //wait until the player is moving away from the grapple point
        yield return new WaitUntil(() => Vector2.Distance(transform.position, grapplePoint) > 0.2f);

        if(atGrapplePoint && !isGrappling){
            //return gravity to the player
            playerRigid.gravityScale = playerGravity;
            atGrapplePoint = false; //player is no longer at the grapple point
        }

    }

    private bool CheckIfGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, GCRadius, whatIsGround);
        return colliders.Length > 0;
    }

    /// <summary>
    /// Animate the transition between two scenes
    /// </summary>
    /// <param name="sceneName">The name of the scene</param>
    /// <returns></returns>
    public IEnumerator LoadSceneAnimation(string sceneName){
        sceneTransitionAnimator.SetBool("LoadingScene", true);

        yield return new WaitUntil(() => sceneTransitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("OnScreen"));

        SceneManager.LoadScene(sceneName);
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(-100f, deathZoneY), new Vector2(100f, deathZoneY));
        // Gizmos.DrawWireSphere((Vector2)keyboardController.gameObject.transform.position + new Vector2(transform.localScale.x, 0f), sideAttack.AttackDamageRange);
        // Gizmos.DrawWireSphere((Vector2)keyboardController.gameObject.transform.position + new Vector2(transform.localScale.x, 0.8f), upAttack.AttackDamageRange);
        // Gizmos.DrawWireSphere((Vector2)keyboardController.gameObject.transform.position + new Vector2(transform.localScale.x, -0.8f), downAttack.AttackDamageRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.transform.position, GCRadius);
    }
}
