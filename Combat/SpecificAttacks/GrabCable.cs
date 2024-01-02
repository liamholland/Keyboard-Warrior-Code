using System.Collections;
using UnityEngine;

public class GrabCable : Attack
{
    [Header("-- Grab Cable --")]
    public Enemy boss;  //reference to the boss
    public GameObject cableEnd; //the game object which the boss actually "throws"
    public LayerMask whatIsGrabbableCable;   //cable physics layer
    public GameObject player;   //reference to the player

    public override void DoAttack(Collider2D colliderToDamage)
    {
        if(colliderToDamage == null) return;

        //spawn the cable
        GameObject cableEndInstance = Instantiate(cableEnd, transform.position, Quaternion.identity);
        cableEndInstance.GetComponent<Cable>().target = colliderToDamage.transform.position;
    }

    public override void WindUpAnimation()
    {
        //assume that it has already been checked that there is at least one cable and that is why the attack is happening

        //find all the cables
        Collider2D[] cables = Physics2D.OverlapCircleAll(transform.position, 50f, whatIsGrabbableCable);

        //pick the one furthest from the player
        float longestDist = 0f;
        Collider2D furthestCable = cables[0];

        foreach(Collider2D cable in cables){
            float distToPlayer = Vector2.Distance(cable.gameObject.transform.position, player.transform.position);

            if(distToPlayer > longestDist){
                longestDist = distToPlayer;
                furthestCable = cable;
            }
        }

        StartCoroutine(PrepareGrabCable(furthestCable.gameObject));
    }

    //go and get the cable
    private IEnumerator PrepareGrabCable(GameObject cable){
        boss.EnemyTarget = new Vector2(cable.transform.position.x, transform.position.y);    //set the boss target

        //wait until the boss has reached the cable
        while( Mathf.Abs(cable.transform.position.x - transform.position.x) > 1f) { yield return null; }

        animator.SetBool(windupAnimationCondition, true); //do the wind up animation

        Destroy(cable); //the cable was ripped out
    }
}
