using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{   
    public Animator animator;   //animator of the entity attacking
    public bool useAttackAnimation = false;
    public string attackState;  //name of the attack state that contains the animation of this attack
    public string attackAnimationCondition; //the name of the condition to change to allow the attack animation to play
    public bool useWindUpAnimation = false;
    public string windupState;  //the name of the wind up animation state
    public string windupAnimationCondition; //the name of the condition to change to allow the windup animation to play

    [SerializeField] private float attackRange; //the range of the attack
    [SerializeField] private int attackDamage;  //the damage the attack does
    [SerializeField] private float attackMoveSpeed; //the speed the enemy moves when attacking

    public float AttackRange => attackRange;    //public accessor for attack range
    public float AttackMoveSpeed => attackMoveSpeed;    //public accessor for move speed

    void Update(){
        if(useAttackAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(attackState)){
            animator.SetBool(attackAnimationCondition, false);
        }

        if(useWindUpAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(windupState)){
            animator.SetBool(windupAnimationCondition, false);
        }
    }

    //play the windup animation
    public void WindUpAnimation(){
        animator.SetBool(windupAnimationCondition, true);
    }

    //do the actual attack
    public virtual void DoAttack(Collider2D colliderToDamage){
        if(useAttackAnimation){
            animator.SetBool(attackAnimationCondition, true);
        }
        
        //do damage to the player collider
        if(colliderToDamage != null){
            colliderToDamage.gameObject.GetComponent<Damageable>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
