using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public bool airControl;
    public CameraController cameraController;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHoldDuration;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float GCRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private SpriteRenderer playerRenderer;
    [Range(25f, 50f)] [SerializeField] private float dashSpeed;
    [Range(0f, 10f)] [SerializeField] private float dashCoolDown;
    [Range(0f, 0.5f)] [SerializeField] private float dashTime;  //the amount of time the dash lasts for
    [Range(0f, 0.5f)] [SerializeField] private float dashHangTime;    //the amount of time you hang in the air after a dash
    [Range(0f, 3f)] [SerializeField] private float dashXShakeMagnitude;  //the magnitude of the dash on the x axis
    [SerializeField] [Range(0f, 0.1f)] private float dashShakeTime;    //the longest time the camera can shake for at once
    [SerializeField] private float dashShakeSpeed;  //the speed of the camera when shaking during a dash
    [SerializeField] private float deathZoneY;  //the y below which the player is dead
    [SerializeField] private float respawnZoneY; //the y at which the player will be respawned

    [SerializeField] private Color dashColor;

    private bool jumpAvailable = true;
    private bool dashAvailable = true;
    private bool isDashing = false;
    private float lastGroundedAt = -1f; //essentially a timer for how long the player has been in the air

    void Update(){
        //make jump available when the key is released
        if(Input.GetButtonUp("Jump")){
            jumpAvailable = true;
        }
    }

    void FixedUpdate()
    {
        //if the player fell off the map
        if(transform.position.y < deathZoneY){
            transform.position = new Vector2(0f, respawnZoneY);
        }
        else if (!Npc.isTalking){   //the player cannot move if they are talking to an npc
            Move();
        }

        if(Input.GetAxisRaw("Horizontal") > 0f){
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if(Input.GetAxisRaw("Horizontal") < 0f){
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    private void Move() 
    {
        if(isDashing){
            return;
        }

        bool grounded = CheckIfGrounded();

        if (grounded || airControl){
            playerRigid.velocity = new Vector2(maxMoveSpeed * Input.GetAxis("Horizontal"), playerRigid.velocity.y);   //set the velocity of the player
        }

        //set the last grounded time if the player is allowed to jump
        if(grounded && jumpAvailable){
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
        else if(Input.GetButton("Dash") && dashAvailable){
            StartCoroutine(Dash());
        }
    }

    //Dash Ability
    private IEnumerator Dash(){
        //the player is now dashing
        dashAvailable = false;
        isDashing = true;

        //change the player's gravity to 0
        float playerGravity = playerRigid.gravityScale;
        playerRigid.gravityScale = 0f;

        //change the color of the player's sprite
        Color playerColour = playerRenderer.color;
        playerRenderer.color = dashColor;
        
        //apply the dash
        playerRigid.velocity = new Vector2(dashSpeed * transform.localScale.x, dashSpeed * Input.GetAxisRaw("Vertical") * 0.8f);

        //shake the camera - applied only in the direction opposite to the direction of the dash
        StartCoroutine(cameraController.ShakeCamera(dashShakeSpeed, dashShakeTime, new Vector2(transform.localScale.x * dashXShakeMagnitude, Input.GetAxisRaw("Vertical")) * 0.4f, Vector2.zero));

        //dash in progress
        yield return new WaitForSeconds(dashTime);

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

    private bool CheckIfGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, GCRadius, whatIsGround);
        return colliders.Length > 0;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector2(0f, respawnZoneY), 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(0f, deathZoneY), 2f);
    }
}
