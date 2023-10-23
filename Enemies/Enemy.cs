using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("-- Target Tracking --")]
    public bool isHostile;  //is the enemy hostile
    public LayerMask whatIsPlayer;  //layermask to identify the player
    public GameObject player;   //what is the player
    [SerializeField] private float playerEscapeRange;   //the distance the player must reach for this enemy to become passive again


    [Header("-- Movement --")]
    public Vector2[] patrolPoints;  //points the enemy will patrol on when it is not hostile
    [SerializeField] private float passiveMovespeed;
    [SerializeField] private float hostileMovespeed;
    public bool getClose = true;    //should the enemy get close to attack
    public LayerMask whatIsObstacle;  //layermask to identify the ground for pathfinding
    public bool canMoveIn2D = false;    //can the enemy move in 2 dimensions
    [Header("If canMoveIn2D = true")]
    [SerializeField] private float pathScanningInterval;    //the amount the enemy scans will move left and right when it detects a collision#
    [SerializeField] private float spaceForEnemy;   //the size of a space the pathfinding needs to look for
    [SerializeField] [Range(0.5f, 2f)] private float maxPathFindingTime;  //the upper limit on the amount of time that can be spent pathfinding


    [Header("-- Combat --")]
    public Attack mainAttack;   //the attack the enemy performs


    [Header("-- Other --")]
    public Animator enemyAnimator;  //the animator of the enemy

    private bool passiveMoveRoutineActive = false;
    private float currentMoveSpeed;
    private bool isAttacking = false;
    private Vector2 currentTarget;
    public Vector2 EnemyTarget {
        set{
            currentTarget = value;
        }
    }

    void Update(){
        PointTowardsTarget();
        //move the enemy towards a target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, currentMoveSpeed * Time.deltaTime);
        // if(gameObject.name.Equals("FlyingEnemy6") && isAttacking){
        //     Debug.Log(Vector2.MoveTowards(transform.position, currentTarget, currentMoveSpeed * Time.deltaTime));
        // }
    }

    void FixedUpdate()
    {
        if(isAttacking) return;

        if (isHostile)
        {
            StopCoroutine(Passive());

            if(TargetWithinAttackRange(player.transform.position)){
                isAttacking = true;
                StartCoroutine(Attack());
            }
            else{
                //set the target to the player
                currentTarget = canMoveIn2D ? FindPathXY(player.transform.position) : FindPathX(player.transform.position);

                //set the current move speed to hostileMoveSpeed
                currentMoveSpeed = hostileMovespeed;

                //if the player has escaped the enemy
                if(Vector2.Distance(transform.position, currentTarget) > playerEscapeRange){
                    isHostile = false;
                    return;
                }
            }
        }
        else
        {
            //set the current speed to passiveMoveSpeed
            currentMoveSpeed = passiveMovespeed;

            //otherwise it should do its passive behaviour
            if (!passiveMoveRoutineActive)
            {
                StartCoroutine(Passive());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("keyboard")){
            //stop the passive coroutine
            StopCoroutine(Passive());
            passiveMoveRoutineActive = false;

            isHostile = true;   //the enemy is hostile
        }
    }

    //path finding for enemies which can only move left and right on the ground
    private Vector2 FindPathX(Vector2 toPoint){
        if(getClose) return toPoint;    //just go towards the point if the enemy needs to get close

        //find a point an acceptable distance from the target (greater than the attack range)

        //get the direction to go
        Vector2 direction = (toPoint - (Vector2)transform.position).normalized * -1;

        int nextDist = 1; //the distance to check if there is a valid point

        Vector2 pointToGoTo = transform.position;    //the point to go to

        //while the target is too close
        while(!TargetWithinAttackRange(pointToGoTo)){
            //the potential point to travel to
            pointToGoTo = (Vector2)transform.position + (direction * nextDist);

            nextDist += nextDist;   //compounds the longer a point is not found
        }

        //a potential point has been found

        //scan in the direction of the potential point
        RaycastHit2D scan = Physics2D.Raycast(transform.position, direction, Vector2.Distance(transform.position, pointToGoTo), whatIsObstacle);

        //if there is an obstacle in the way
        if(scan.collider != null){
            //change the point to a little bit away from whatever obstacle was hit
            pointToGoTo = new Vector2(scan.point.x - 3f, scan.point.y);
        }

        //return the point
        return pointToGoTo;
    }


    //path finding for 2D, flying enemies
    private Vector2 FindPathXY(Vector2 toPoint)
    {
        //check if there is ground in the way
        RaycastHit2D ground = Physics2D.Raycast(transform.position, toPoint - (Vector2)transform.position, Vector2.Distance(transform.position, toPoint), whatIsObstacle);

        //return the point that was passed if there is nothing in the way
        if(ground.collider == null) return toPoint;

        //otherwise, find a temporary point to travel to to avoid the ground

        //initialise a left and right point at the point of the collision
        Vector2 leftPoint = ground.point;
        Vector2 rightPoint = ground.point;

        //declare two scanners
        RaycastHit2D leftScan;
        RaycastHit2D rightScan;
        
        float elapsedTime = 0f;

        //scan left and right until there is no ground or the maximum time is exceeded
        while(elapsedTime < maxPathFindingTime){
            //move the target of the left and right scanners more left and right
            leftPoint = new Vector2(leftPoint.x - pathScanningInterval, leftPoint.y);
            rightPoint = new Vector2(rightPoint.x + pathScanningInterval, rightPoint.y);

            //do a scan to the left and to the right
            leftScan = Physics2D.Raycast(transform.position, leftPoint - (Vector2)transform.position, Vector2.Distance(transform.position, leftPoint), whatIsObstacle);
            rightScan = Physics2D.Raycast(transform.position, rightPoint - (Vector2)transform.position, Vector2.Distance(transform.position, rightPoint), whatIsObstacle);

            //if there is a space, check that there is enough space for the enemy, if so return the point
            if(leftScan.collider == null){
                Collider2D groundNearHit = Physics2D.OverlapCircle(leftPoint, spaceForEnemy, whatIsObstacle);

                if(groundNearHit == null) return leftPoint;
            }
            else if(rightScan.collider == null){
                Collider2D groundNearHit = Physics2D.OverlapCircle(rightPoint, spaceForEnemy, whatIsObstacle);

                if(groundNearHit == null) return rightPoint;
            }

            elapsedTime += Time.deltaTime;
        }

        return transform.position;  //something has gone wrong, return the enemy's position
    }

    //point the enemy towards the target it is heading for
    private void PointTowardsTarget(){
        if(currentTarget.x > transform.position.x){
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else{
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    //the attack behaviour of the enemy
    private IEnumerator Attack()
    {   
        //the enemy is attacking
        isAttacking = true;

        //set the move speed
        currentMoveSpeed = mainAttack.WindUpMoveSpeed;

        mainAttack.WindUpAnimation();   //run the windup animation

        //wind up on the enemies attack
        yield return new WaitUntil(() => enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        // Debug.Log("Doing attack");
        //find all the player colliders in range of the enemy after the wind up
        Collider2D colliderInRange = Physics2D.OverlapCircle(transform.position, mainAttack.AttackRange, whatIsPlayer);

        //set the move speed
        currentMoveSpeed = mainAttack.AttackMoveSpeed;

        //execute the attack
        mainAttack.DoAttack(colliderInRange);

        //wait until the animation for the attack is done
        yield return new WaitUntil(() => enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        currentMoveSpeed = mainAttack.CooldownMoveSpeed;

        // Debug.Log("Cooldown");
        yield return new WaitForSeconds(mainAttack.AttackCoolDown);

        //reset the move speed to be hostile
        currentMoveSpeed = hostileMovespeed;
        // Debug.Log("move speed back to hostile");

        //the enemy is no longer attacking
        isAttacking = false;
        // Debug.Log("Attack Finished");
    }

    //is the target within the attack range of the enemy
    private bool TargetWithinAttackRange(Vector2 targetPosition){
        if(getClose){
            return Vector2.Distance(transform.position, targetPosition) < mainAttack.AttackRange;
        }
        else{
            return Vector2.Distance(transform.position, targetPosition) > mainAttack.AttackRange;
        }
    }

    //default implementation of a passive action
    //patrols around the patrol points
    private IEnumerator Passive()
    {
        passiveMoveRoutineActive = true;

        //for each point provided
        foreach (Vector2 point in patrolPoints)
        {
            while(true){
                //set the current target to the point or a temporary point to avoid obstacles
                currentTarget = canMoveIn2D ? FindPathXY(point) : point;

                //if the pathfinding returned a temporary point, travel to that point and try again
                if(Equals(point, currentTarget)){
                    break;
                }
                else{
                    yield return new WaitUntil(() => Vector2.Distance(currentTarget, transform.position) < 0.4f);
                }
            }

            yield return new WaitUntil(() => Vector2.Distance(point, transform.position) < 0.4f);
        }

        //the routine is finished
        passiveMoveRoutineActive = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach(Vector2 point in patrolPoints){
            Gizmos.DrawWireSphere(point, 0.4f);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, currentTarget);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerEscapeRange);
    }
}
