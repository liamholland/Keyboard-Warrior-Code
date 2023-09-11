using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public GameObject groundCheck;
    public float GCRadius;
    public LayerMask whatIsGround;
    public Rigidbody2D playerRigid;
    public BoxCollider2D playerCollider;
    public bool airControl;
    
    void FixedUpdate()
    {
        if (!Npc.isTalking)
        {
            Move();
        }
    }

    private void Move()
    {
        if (CheckIfGrounded() || airControl)
        {
            playerRigid.velocity = new Vector2(moveSpeed * Input.GetAxisRaw("Horizontal"), playerRigid.velocity.y);
        }

        if(CheckIfGrounded() && Input.GetAxisRaw("Vertical") > 0)
        {
            playerRigid.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private bool CheckIfGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, GCRadius, whatIsGround);
        return colliders.Length > 0;
    }
}
