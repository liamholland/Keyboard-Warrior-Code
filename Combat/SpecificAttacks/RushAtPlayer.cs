using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushAtPlayer : Attack
{
    [SerializeField] private float dashDistance;    //the distance the attack goes
    [SerializeField] private LayerMask dashObstacles;  //things the dash cannot go through

    private GameObject player;  //reference to the player
    private Enemy enemy; //reference to the enemy

    private void Start(){
        //set the player reference
        player = GameObject.Find("Player");
        
        //set the enemy reference
        enemy = gameObject.GetComponent<Enemy>();
    }

    public override void DoAttack(Collider2D colliderToDamage)
    {
        animator.SetBool(attackAnimationCondition, true);

        attackSound.Play();

        StartCoroutine(DashAttack());
    }

    private IEnumerator DashAttack(){
        Vector2 target = transform.position + ((player.transform.position - transform.position).normalized * dashDistance);
        
        RaycastHit2D scan = Physics2D.Raycast(transform.position, target - (Vector2)transform.position, dashDistance, dashObstacles);

        if(scan.collider != null){
            target = (Vector2)transform.position + ((scan.point - (Vector2)transform.position).normalized * (Vector2.Distance(transform.position, scan.point) - 2f));
        }

        enemy.EnemyTarget = target;

        yield return new WaitUntil(() => Vector2.Distance(target, transform.position) < 0.3f ||
                                    Vector2.Distance(player.transform.position, transform.position) < AttackDamageRange);

        if(Vector2.Distance(player.transform.position, transform.position) < AttackDamageRange){
            player.GetComponent<Damageable>().TakeDamage(AttackDamage);
        }

        animator.SetBool(attackAnimationCondition, false);
    }
}
