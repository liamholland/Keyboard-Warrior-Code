using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isHostile;  //is the enemy hostile
    public int health;
    public Vector2[] patrolPoints;  //points the enemy will patrol on when it is not hostile
    public float passiveMovespeed;
    public float hostileMovespeed;
    public LayerMask whatIsPlayer;  //layermask to identify the player
    [Range(0f, 0.5f)] public float damageFlashTime;   //amount of time the enemy flashes for damage
    public Color damageFlashColor;  //the color that the enemy flashes when it takes damage


    private Rigidbody2D enemyRigid;
    private SpriteRenderer enemyRenderer;
    private bool passiveMoveRoutineActive = false;
    private GameObject player;
    private Vector2 currentTarget;
    private float currentMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //get the rigidbody of the enemy
        enemyRigid = GetComponent<Rigidbody2D>();

        //get the renderer of the enemy
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    void Update(){
        if(currentTarget.x > transform.position.x){
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else{
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        
        //move the enemy towards a target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, currentMoveSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isHostile)
        {
            //set the current move speed to hostileMoveSpeed
            currentMoveSpeed = hostileMovespeed;

            //set the target to the player
            currentTarget = player.transform.position;
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
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            //stop the passive coroutine
            StopCoroutine(Passive());
            
            isHostile = true;   //the enemy is hostile

            player = other.gameObject;  //set the reference to the player
        }
    }

    //the attack behaviour of the enemy
    public void Attack()
    {   

    }

    //default implementation of a passive action
    //patrols around the patrol points
    public IEnumerator Passive()
    {
        passiveMoveRoutineActive = true;

        //for each point provided
        foreach (Vector2 point in patrolPoints)
        {
            //get the distance between the enemy and the point
            //cannot use Vector2.Distance() because i need to know if it's negative
            float differenceInPosition = point.x - transform.position.x;

            //direction of the point relevant to the enemy
            float direction = differenceInPosition / Mathf.Abs(differenceInPosition);

            //set the current target to the point
            currentTarget = point;

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
    }
}
