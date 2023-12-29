using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonColumns : Attack
{
    public GameObject column;  //the columns to summon
    public GameObject columnMarker;    //the game objects which indicate where the columns will spawn
    public Enemy enemy;

    private List<GameObject> columnMarkers = new List<GameObject>();

    //delay object
    private static WaitForSeconds oneSecondDelay = new WaitForSeconds(1f);

    public override void WindUpAnimation()
    {
        StartCoroutine(PrepareColumns());
    }

    public override void DoAttack(Collider2D colliderToDamage)
    {
        //spawn columns
        foreach(GameObject marker in columnMarkers){            
            Destroy(Instantiate(column, marker.transform.position, Quaternion.identity), 1f);
            Destroy(marker);
        }

        columnMarkers.Clear();   //reset the list
    }

    //wind up
    private IEnumerator PrepareColumns(){
        Vector2 arenaCenter = new Vector2(43f, 6f);

        enemy.EnemyTarget = arenaCenter;   //set the enemy target to the middle of the arena

        //wait until the boss reaches the middle of the arena
        yield return new WaitUntil(() => Vector2.Distance(transform.position, arenaCenter) < 0.3f);

        animator.SetBool(windupAnimationCondition, true);

        int numColumns = Random.Range(5, 7);

        //instantiate column markers
        for(int i = 0; i < numColumns; i++){
            columnMarkers.Add(Instantiate(columnMarker, new Vector3(transform.position.x - (26f / 2f) + (i * (26f / numColumns)), 2f, 0f), Quaternion.identity));
        }

        yield return oneSecondDelay;

        animator.SetBool(attackAnimationCondition, true);
    }
}
