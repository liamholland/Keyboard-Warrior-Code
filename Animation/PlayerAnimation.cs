using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;   //reference to the player's animator

    // Update is called once per frame
    void Update()
    {
        //play jump animation when player jumps
        if(Input.GetButtonDown("Jump")){
            playerAnimator.SetBool("jumping", true);
        }
        //if player is moving, play the move animation
        else if(Input.GetAxisRaw("Horizontal") != 0){
            playerAnimator.SetBool("moving", true);
        }
        else{
            playerAnimator.SetBool("moving", false);
        }

        //if the jump animation is playing, stop it from playing again
        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump")){
            playerAnimator.SetBool("jumping", false);
        }
    }
}
