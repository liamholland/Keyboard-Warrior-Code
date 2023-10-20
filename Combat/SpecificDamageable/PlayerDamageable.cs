using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable
{
    [Range(0f, 0.5f)] [SerializeField] private float stunTime;  //the amount of time the player is stunned for

    //when the player takes damage
    public override void TakeDamage(int damage)
    {
        health -= damage;   //take damage

        if(health <= 0){
            gameObject.SetActive(false);    //disable the player when made inactive
        }
        else{
            Debug.Log(health);

            //if there is a stun, apply the affect
            if(stunTime > 0f){
                StartCoroutine(Stun());
            }
        }
    }

    //apply a stun affect to the player
    private IEnumerator Stun(){
        Controller.interacting = true;

        yield return new WaitForSeconds(stunTime);

        Controller.interacting = false;
    }
}
