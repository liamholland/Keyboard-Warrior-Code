using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public bool isHostile;  //is the enemy hostile
    public int health;
    public Vector2[] patrolPoints;  //points the enemy will patrol on when it is not hostile
    public float detectionRange;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        if(isHostile){
            //if the enemy is hostile it should attack the player   
            Attack();
        }
        else{
            //otherwise it should do its passive behaviour
        }
    }

    public abstract void Attack();  //the attack behaviour of the enemy

    public abstract void Passive(); //what the enemy does when it is not hostile
}
