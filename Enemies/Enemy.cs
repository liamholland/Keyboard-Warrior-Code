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
    private bool isCooldown = false;
    private Vector2 currentTarget;
    private Coroutine runningPassive;   //the running passive coroutine
    public Vector2 EnemyTarget {
        set{
            currentTarget = value;
        }
    }

    void Update(){
        PointTowardsTarget();
        //move the enemy towards a target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, currentMoveSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isHostile)
        {
            //if there is a running passive coroutine, stop it
            if(runningPassive != null) StopCoroutine(runningPassive);

            if(!isAttacking && TargetWithinAttackRange(player.transform.position)){
                isAttacking = true;
                ChooseAttack();
            }
            else if(!isAttacking){
                //set the current move speed to hostileMoveSpeed
                currentMoveSpeed = hostileMovespeed;

                //if the player has escaped the enemy
                if(Vector2.Distance(transform.position, currentTarget) > playerEscapeRange){
                    isHostile = false;
                    return;
                }
            }

            if(!isAttacking || isCooldown){
                //use the correct pathfinding algorithm based on whether the enemy should get close to attack
                currentTarget = getClose ? FindPathToTarget(player.transform.position) : FindPathAwayFromTarget(player.transform.position);
            }
        }
        else
        {
            //set the current speed to passiveMoveSpeed
            currentMoveSpeed = passiveMovespeed;

            //otherwise it should do its passive behaviour
            if (!passiveMoveRoutineActive)
            {
                runningPassive = StartCoroutine(Passive());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("keyboard")){
            passiveMoveRoutineActive = false;   //stop the passive coroutine

            isHostile = true;   //the enemy is hostile
        }
    }

    //path finding for enemies which can only move left and right on the ground
    protected virtual Vector2 FindPathAwayFromTarget(Vector2 toPoint){
        //find a point an acceptable distance from the target (greater than the attack range)

        if(TargetWithinAttackRange(toPoint)){
            return transform.position;
        }

        //get the direction to go is opposite to the direction of the point
        Vector2 direction = new Vector2((toPoint.x - transform.position.x) * -1, canMoveIn2D ? 5f : 0f).normalized;

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
            //get the distance between the target and the enemy
            //the float constant is an arbitrary value
            //once the target is within this constant range of the enemy, it will begin to move away from whatever obstacle it is next to
            float distToPoint = 3f - Vector2.Distance(transform.position, toPoint);

            //change the point to a little bit away from whatever obstacle was hit
            //if the target is within the range of distToPoint (i.e. distToPoint not negative), the enemy has been cornered and should run past the target to escape
            //so, the point will be the scan point minus 2 plus the inversely proportional distance between the enemy and the target
            pointToGoTo = (Vector2)transform.position + (direction * (Vector2.Distance(scan.point, transform.position) - (2f + (distToPoint > 0 ? distToPoint : 0f))));
        }

        //return the point
        return new Vector2(pointToGoTo.x, canMoveIn2D ? pointToGoTo.y : transform.position.y);
    }


    //path finding for 2D, flying enemies
    protected virtual Vector2 FindPathToTarget(Vector2 toPoint)
    {
        if(canMoveIn2D){
            //check if there is ground in the way
            RaycastHit2D ground = Physics2D.Raycast(transform.position, (toPoint - (Vector2)transform.position).normalized, Vector2.Distance(transform.position, toPoint), whatIsObstacle);

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
        else{
            return toPoint;
        }
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

    /// <summary>
    /// Choose the attack the enemy will use
    /// </summary>
    public virtual void ChooseAttack(){
        StartCoroutine(Attack(mainAttack));
    }

    /// <summary>
    /// The attack behaviour of the enemy
    /// </summary>
    /// <returns></returns>
    public IEnumerator Attack(Attack attack)
    {   
        //the enemy is attacking
        isAttacking = true;

        //set the move speed
        currentMoveSpeed = attack.WindUpMoveSpeed;

        attack.WindUpAnimation();   //run the windup animation

        //wind up on the enemies attack
        while(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(attack.attackState != "" ? attack.attackState : "Attack")){
            yield return null;
        }

        // Debug.Log("Doing attack");
        //find all the player colliders in range of the enemy after the wind up
        Collider2D colliderInRange = Physics2D.OverlapCircle(transform.position, attack.AttackDamageRange, whatIsPlayer);

        //set the move speed
        currentMoveSpeed = attack.AttackMoveSpeed;

        //execute the attack
        attack.DoAttack(colliderInRange);

        //wait until the animation for the attack is done
        while(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
            yield return null;
        }
        
        //the enemy is on cooldown
        isCooldown = true;


        currentMoveSpeed = attack.CooldownMoveSpeed;

        // Debug.Log("Cooldown");
        yield return new WaitForSeconds(attack.AttackCoolDown);

        //the enemy is out of cooldown
        isCooldown = false;

        //reset the move speed to be hostile
        currentMoveSpeed = hostileMovespeed;

        //the enemy is no longer attacking
        isAttacking = false;
    }

    //is the target within the attack range of the enemy
    private bool TargetWithinAttackRange(Vector2 targetPosition){
        if(getClose){
            return Vector2.Distance(transform.position, targetPosition) < mainAttack.AttackTriggerRange;
        }
        else{
            return Vector2.Distance(transform.position, targetPosition) > mainAttack.AttackTriggerRange;
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
                currentTarget = FindPathToTarget(point);

                //if the pathfinding returned a temporary point, travel to that point and try again
                if(Equals(point, currentTarget)){
                    break;
                }
                else{
                    //wait until the enemy is within 0.4 units of the point
                    //wait until object is necessary here as using a regular while loop causes an infinite loop for some reason
                    //i think it might be due to the fact that WaitUntil is guaranteed to wait at least one frame
                    //so if you use a while loop and the condition is met, but the pathfinding returns the same point, you are basically stuck in the outer loop
                    yield return new WaitUntil(() => Vector2.Distance(currentTarget, transform.position) < 0.4f);
                }
            }

            //wait until the enemy is within 0.4 units of the point
            while(Vector2.Distance(point, transform.position) >= 0.4f) { yield return null; }
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
