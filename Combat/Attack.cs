using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{   
    public Animator animator;   //animator of the entity attacking
    [Header("-- Wind Up --")]
    public bool useAutoWindUpAnimation = false;
    public string windupState;  //the name of the wind up animation state
    public string windupAnimationCondition; //the name of the condition to change to allow the windup animation to play
    [SerializeField] private float windUpMoveSpeed; //the speed of an enemies windup
    [SerializeField] protected AudioSource windUpSound;   //the sound used to signpost the attack

    [Header("-- Attack --")]
    public bool useAutoAttackAnimation = false;
    public string attackState;  //name of the attack state that contains the animation of this attack
    public string attackAnimationCondition; //the name of the condition to change to allow the attack animation to play
    [SerializeField] private float attackTriggerRange; //the range of the attack
    [SerializeField] private float attackDamageRange;
    [SerializeField] private int attackDamage;  //the damage the attack does
    [SerializeField] private float attackMoveSpeed; //the speed the enemy moves when attacking
    [SerializeField] protected AudioSource attackSound;   //the sound played when the attack happens

    [Header("-- Cooldown --")]
    [SerializeField] private float attackCoolDown;    //amount of time the enemy must wait for
    [SerializeField] private float coolDownMoveSpeed;   //how fast the entity moves when on cooldown

    public float AttackTriggerRange => attackTriggerRange;    //public accessor for trigger range
    public float AttackDamageRange => attackDamageRange;    //public accessor for damage range
    public int AttackDamage => attackDamage;    //public accessor for the damage
    public float AttackMoveSpeed => attackMoveSpeed;    //public accessor for attack move speed
    public float WindUpMoveSpeed => windUpMoveSpeed;    //public accessor for wind up move speed
    public float AttackCoolDown => attackCoolDown;  //public accessor for attack cooldown
    public float CooldownMoveSpeed => coolDownMoveSpeed;    //public accessor for cooldown move speed
    public int AttackCost { get; set; } //the amount the attack costs - Used for choosing an attack to use - not used in default implementation

    void Update(){
        if(useAutoAttackAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(attackState)){
            animator.SetBool(attackAnimationCondition, false);
        }

        if(useAutoWindUpAnimation && animator.GetCurrentAnimatorStateInfo(0).IsName(windupState)){
            animator.SetBool(windupAnimationCondition, false);
        }
    }

    //play the windup animation
    public virtual void WindUpAnimation(){
        if(useAutoWindUpAnimation){
            animator.SetBool(windupAnimationCondition, true);
        }

        if(windUpSound != null){
            windUpSound.Play();
        }
    }

    //do the actual attack
    public virtual void DoAttack(Collider2D colliderToDamage){
        if(useAutoAttackAnimation){
            animator.SetBool(attackAnimationCondition, true);
        }

        if(attackSound != null){
            attackSound.Play();
        }
        
        //do damage to the player collider
        if(colliderToDamage != null){
            colliderToDamage.gameObject.GetComponent<Damageable>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackTriggerRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDamageRange);
    }
}
