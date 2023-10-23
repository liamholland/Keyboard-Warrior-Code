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
    [SerializeField] private float windUpMoveSpeed; //the speed of an enemies windup
    [SerializeField] private float attackMoveSpeed; //the speed the enemy moves when attacking
    [SerializeField] private float attackCoolDown;    //amount of time the enemy must wait for
    [SerializeField] private float coolDownMoveSpeed;   //how fast the entity moves when on cooldown

    public float AttackRange => attackRange;    //public accessor for attack range
    public int AttackDamage => attackDamage;    //public accessor for the damage
    public float AttackMoveSpeed => attackMoveSpeed;    //public accessor for attack move speed
    public float WindUpMoveSpeed => windUpMoveSpeed;    //public accessor for wind up move speed
    public float AttackCoolDown => attackCoolDown;  //public accessor for attack cooldown
    public float CooldownMoveSpeed => coolDownMoveSpeed;    //public accessor for cooldown move speed

    void Update(){
        if(useAttackAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(attackState)){
            animator.SetBool(attackAnimationCondition, false);
        }

        if(useWindUpAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(windupState)){
            animator.SetBool(windupAnimationCondition, false);
        }
    }

    //play the windup animation
    public virtual void WindUpAnimation(){
        if(useWindUpAnimation){
            animator.SetBool(windupAnimationCondition, true);
        }
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
