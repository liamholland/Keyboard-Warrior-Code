using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float maxMoveSpeed;
    public float jumpForce;
    public float jumpHoldDuration;
    public GameObject groundCheck;
    public float GCRadius;
    public LayerMask whatIsGround;
    public Rigidbody2D playerRigid;
    public BoxCollider2D playerCollider;
    public SpriteRenderer playerRenderer;
    public bool airControl;
    [Range(25f, 50f)] public float dashSpeed;
    [Range(0f, 10f)] public float dashCoolDown;
    [Range(0f, 0.5f)] public float dashTime;  //the amount of time the dash lasts for
    [Range(0f, 0.5f)] public float dashHangTime;    //the amount of time you hang in the air after a dash
    public Color dashColor;

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
        //the player cannot move if they are talking to an npc
        if (!Npc.isTalking){
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
        playerRigid.velocity = new Vector2(dashSpeed * transform.localScale.x, dashSpeed * Input.GetAxisRaw("Vertical") * 0.5f);

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
}
