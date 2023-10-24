using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Attack
{
    public GameObject projectile;   //the project to shoot
    public GameObject player;   //reference to the player
    [SerializeField] private float projectileSpeed; //the speed to shoot the projectile at

    public override void DoAttack(Collider2D colliderToDamage)
    {
        if(useAttackAnimation){
            animator.SetBool(attackAnimationCondition, true);
        }

        //point towards target
        transform.localScale = new Vector2((player.transform.position - transform.position).normalized.x, transform.localScale.y);

        //shoot a projectile
        GameObject shotProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        //set the speed
        shotProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * projectileSpeed, 0f);
        
        //set the damage
        shotProjectile.GetComponent<Projectile>().damage = AttackDamage;

        //set the destroy time
        shotProjectile.GetComponent<Projectile>().destroyTime = 5f;
    }
}
