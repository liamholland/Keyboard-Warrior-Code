using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isHostile;  //is the enemy hostile
    public Vector2[] patrolPoints;  //points the enemy will patrol on when it is not hostile
    public LayerMask whatIsPlayer;  //layermask to identify the player
    public LayerMask whatIsObstacle;  //layermask to identify the ground for pathfinding
    public GameObject player;   //what is the player

    [SerializeField] private int health;
    [SerializeField] private float passiveMovespeed;
    [SerializeField] private float hostileMovespeed;
    [Range(0f, 0.5f)] [SerializeField] private float damageFlashTime;   //amount of time the enemy flashes for damage
    [SerializeField] private Color damageFlashColor;  //the color that the enemy flashes when it takes damage
    [SerializeField] private float attackRange; //the range the enemy can attack from
    [SerializeField] private float attackWindUpDuration;    //amount of time the enemy winds up their attack for
    [SerializeField] private Color attackFlashColor;  //the color that the enemy flashes when it attacks
    [SerializeField] private Color attackWindUpColor;  //the color that the enemy flashes when it is winding up an attack
    [SerializeField] private float pathScanningInterval;    //the amount the enemy scans will move left and right when it detects a collision#
    [SerializeField] [Range(0.5f, 2f)] private float maxPathFindingTime;  //the upper limit on the amount of time that can be spent pathfinding
    [SerializeField] private float spaceForEnemy;   //the size of a space the pathfinding needs to look for


    private Rigidbody2D enemyRigid;
    private SpriteRenderer enemyRenderer;
    private bool passiveMoveRoutineActive = false;
    private Vector2 currentTarget;
    private float currentMoveSpeed;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        //get the rigidbody of the enemy
        enemyRigid = GetComponent<Rigidbody2D>();

        //get the renderer of the enemy
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    void Update(){
        PointTowardsTarget();
        
        if(!isAttacking){
            //move the enemy towards a target
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, currentMoveSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (isHostile)
        {
            //set the target to the player
            currentTarget = FindPath(player.transform.position);

            if(TargetWithinAttackRange(player.transform.position) && !isAttacking){
                StartCoroutine(Attack());
            }
            else if(!isAttacking){
                //set the current move speed to hostileMoveSpeed
                currentMoveSpeed = hostileMovespeed;
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
            
            isHostile = true;   //the enemy is hostile
        }
    }

    //path finding
    private Vector2 FindPath(Vector2 toPoint)
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

        //stop the enemy
        currentMoveSpeed = 0f;

        Color enemyColor = enemyRenderer.color;

        enemyRenderer.color = attackWindUpColor;

        //wind up on the enemies attack
        yield return new WaitForSeconds(attackWindUpDuration);

        enemyRenderer.color = attackFlashColor;

        //find all the player colliders in range of the enemy after the wind up
        Collider2D colliderInRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);

        //if any colliders were in range
        if(colliderInRange != null){
            colliderInRange.gameObject.GetComponent<Controller>().TakeDamage(1);
        }

        yield return new WaitForSeconds(0.1f);

        //reset the move speed to be hostile
        currentMoveSpeed = hostileMovespeed;

        enemyRenderer.color = enemyColor;

        //the enemy is no longer attacking
        isAttacking = false;

        yield break;
    }

    //is the target within the attack range of the enemy
    private bool TargetWithinAttackRange(Vector2 targetPosition){
        return Vector2.Distance(transform.position, targetPosition) < attackRange;
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
                currentTarget = FindPath(point);

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

    //what the enemy does when it takes damage
    public void TakeDamage(int damage, float knockbackForce)
    {
        health -= damage;   //reduce the enemies health
        
        //make the enemy flash to indicate damage was taken
        StartCoroutine(FlashOnDamage());

        if(health > 0){
            currentTarget = transform.position;
            enemyRigid.AddForce(new Vector2(knockbackForce * (transform.localScale.x * -1), 0.1f)); //add some knockback
            currentTarget = player.transform.position;
        }

    }

    private IEnumerator FlashOnDamage()
    {
        Color enemyColor = enemyRenderer.color;

        //make enemy flash
        enemyRenderer.color = damageFlashColor;

        //delay
        yield return new WaitForSeconds(damageFlashTime);

        //back to the original color
        enemyRenderer.color = enemyColor;

        if(health <= 0){
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach(Vector2 point in patrolPoints){
            Gizmos.DrawWireSphere(point, 0.4f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentTarget);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
