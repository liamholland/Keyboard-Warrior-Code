using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;   //reference to the player's animator

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Attack")){
            playerAnimator.SetBool("attacking", true);
        }
        //play jump animation when player jumps
        else if(Input.GetButtonDown("Jump")){
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
        //if the attack animation is playing stop it from playing again
        else if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack")){
            playerAnimator.SetBool("attacking", false);
        }

    }
}
