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
        if(useAutoAttackAnimation){
            animator.SetBool(attackAnimationCondition, true);
        }

        windUpSound.Stop(); //specific to this attack; the sound i have is too long
        attackSound.Play();

        //point towards target
        transform.localScale = new Vector2((player.transform.position - transform.position).normalized.x, transform.localScale.y);

        //shoot a projectile
        GameObject shotProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

        //set the speed
        shotProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * projectileSpeed, 0f);
        
        //get the projectile component
        Projectile projectileComponent = shotProjectile.GetComponent<Projectile>();

        if(projectileComponent != null){
            //set the damage
            projectileComponent.damage = AttackDamage;

            //set the destroy time
            projectileComponent.destroyTime = 5f;
        }
    }
}
