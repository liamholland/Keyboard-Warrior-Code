using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator animator;   //animator of the entity attacking - assumes they have a parameters "windingUp"

    [SerializeField] private float attackRange; //the range of the attack
    [SerializeField] private int attackDamage;  //the damage the attack does
    [SerializeField] private float attackMoveSpeed; //the speed the enemy moves when attacking

    public float AttackRange => attackRange;    //public accessor for attack range
    public float AttackMoveSpeed => attackMoveSpeed;    //public accessor for move speed

    void Update(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            animator.SetBool("windingUp", false);
        }
    }

    //play the windup animation
    public void WindUpAnimation(){
        animator.SetBool("windingUp", true);
    }

    //do the actual attack
    public virtual void DoAttack(Collider2D colliderToDamage){
        //do damage to the player collider
        if(colliderToDamage != null){
            colliderToDamage.gameObject.GetComponent<Controller>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
