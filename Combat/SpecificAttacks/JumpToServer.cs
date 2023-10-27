using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToServer : Attack
{   
    [Header("-- Jump To Server")]
    public Enemy boss;
    public Rigidbody2D bossRigid;
    public GameObject[] servers;
    public GameObject enemyToSpawn; //the enemy the boss will spawn
    public GameObject player;   //reference to the player for the enemy spawn

    private List<GameObject> enemiesSpawned;   //list of the spawned enemies
    private float bossGravity;

    public override void DoAttack(Collider2D colliderToDamage)
    {   
        enemiesSpawned = new List<GameObject>();

        //spawn 3 enenmies
        StartCoroutine(SpawnEnemies());

        StartCoroutine(WaitForMinionDeaths());
    }

    public override void WindUpAnimation()
    {
        StartCoroutine(GetOnServer());
    }

    private IEnumerator SpawnEnemies(){
        for(int i = 0; i < 3; i++){
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(attackState));

            GameObject enemySpawn = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);

            Enemy eComponent = enemySpawn.GetComponent<Enemy>();
            eComponent.player = player;   //set the player reference
            eComponent.patrolPoints[0] = player.transform.position; //move towards the player

            enemiesSpawned.Add(enemySpawn); //add it to the list of enemies

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(windupState));

            animator.SetBool(attackAnimationCondition, true);
        }
    }

    private IEnumerator WaitForMinionDeaths(){
        int numMinions;
        
        do{
            yield return new WaitForSeconds(1f);

            numMinions = 0;

            //count the number of minions that are alive
            foreach(GameObject enemy in enemiesSpawned){
                if(enemy != null){
                    numMinions++;
                }
            }
        }while(numMinions > 0);

        bossRigid.gravityScale = bossGravity;   //reset the boss rigid body

        //replay the jump animation while the enemy is falling
        animator.SetBool(windupAnimationCondition, false);
    }

    private IEnumerator GetOnServer(){
        Vector2 arenaCenter = new Vector2(43f, 6f);

        boss.EnemyTarget = arenaCenter;

        yield return new WaitUntil(() => Vector2.Distance(transform.position, arenaCenter) < 0.3f);

        //remove gravity on the boss
        bossGravity = bossRigid.gravityScale;
        bossRigid.gravityScale = 0f;

        //start the animation
        animator.SetBool(windupAnimationCondition, true);

        //pick a server position
        Vector2 serverPosition = servers[Random.Range(0, 2)].transform.position;

        boss.EnemyTarget = serverPosition;  //set the position

        yield return new WaitUntil(() => Vector2.Distance(transform.position, serverPosition) < 0.3f);

        boss.EnemyTarget = transform.position;  //stay in place

        animator.SetBool(attackAnimationCondition, true);
    }
}
