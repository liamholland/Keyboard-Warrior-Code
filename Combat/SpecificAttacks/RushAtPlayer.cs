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
        if(useAttackAnimation){
            animator.SetBool(attackAnimationCondition, true);
        }

        // Debug.Log("Attacking");
        StartCoroutine(DashAttack());
    }

    private IEnumerator DashAttack(){
        Vector2 target = transform.position + ((player.transform.position - transform.position).normalized * dashDistance);
        // Debug.Log("Target " + target + " chosen");
        
        RaycastHit2D scan = Physics2D.Raycast(transform.position, target - (Vector2)transform.position, dashDistance, dashObstacles);

        if(scan.collider != null){
            // Debug.Log("Going to hit wall");
            target = (Vector2)transform.position + ((scan.point - (Vector2)transform.position).normalized * (Vector2.Distance(transform.position, scan.point) - 2f));
        }

        enemy.EnemyTarget = target;
        // Debug.Log("Target " + target + " set");
        
        // float distanceToReachTarget = 0.3f; //the starting distance that the enemy should reach the target at

        /*
        POSSIBLY FIXED
        There are a lot of debugs around here because of this
        For some reason, there are cases in which the target is set on the enemy and it just does not move towards the target
        I have not been able to figure out why this is yet
        For now, this workaround prevents the enemy from being stuck indefinitely
        */
        // while(true){
        //     if(Vector2.Distance(transform.position, target) < distanceToReachTarget){
        //         break;
        //     }

        //     yield return new WaitForSeconds(0.5f);

        //     distanceToReachTarget += 1f;
        // }

        yield return new WaitUntil(() => Vector2.Distance(target, transform.position) < 0.3f ||
                                    Vector2.Distance(player.transform.position, transform.position) < AttackRange);
        // Debug.Log("Reached Target");

        if(Vector2.Distance(player.transform.position, transform.position) < AttackRange){
            player.GetComponent<Damageable>().TakeDamage(AttackDamage);
        }

        animator.SetBool(attackAnimationCondition, false);
        // Debug.Log("Done Dashing");
    }
}
