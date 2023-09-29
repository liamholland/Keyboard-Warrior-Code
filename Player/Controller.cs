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

    private bool jumpAvailable = true;
    private float lastGroundedAt = -1f; //essentially a timer for how long the player has been in the air
    private float direction = 0f;
    private bool flip = false;

    void Update(){
        //make jump available when the key is released
        if(Input.GetButtonUp("Jump")){
            jumpAvailable = true;
        }

        playerRenderer.flipX = flip;  //if the player is facing to the left, flip the sprite on the x axis
    }

    void FixedUpdate()
    {
        //the player cannot move if they are talking to an npc
        if (!Npc.isTalking){
            Move();
        }
    }

    private void Move() 
    {
        bool grounded = CheckIfGrounded();

        if (grounded || airControl){
            float currDir = Input.GetAxisRaw("Horizontal");    //get a reference to the current direction the player is moving

            flip = currDir * direction < 0f ? !flip : flip;  //if the directions have flipped (-1 * 0 = 0, -1 * -1 = 1, etc... but only -1 * 1 = -1)

            direction = currDir;    //update the direction

            playerRigid.velocity = new Vector2(maxMoveSpeed * direction, playerRigid.velocity.y);   //set the velocity of the player
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
    }

    private bool CheckIfGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, GCRadius, whatIsGround);
        return colliders.Length > 0;
    }
}
