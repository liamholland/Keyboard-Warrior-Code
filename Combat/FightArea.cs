using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightArea : MonoBehaviour
{
    [SerializeField] private Collider2D area;
    [SerializeField] private LayerMask whatIsEnemy;

    private List<Enemy> enemies = new List<Enemy>();    //list of enemies
    
    // Start is called before the first frame update
    void Start()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapBoxAll(area.bounds.center, area.bounds.size, 0f, whatIsEnemy);
        
        //add enemies to the list
        foreach(Collider2D c in enemyColliders){
            enemies.Add(c.gameObject.GetComponent<Enemy>());
        }
    }

    //the fight is complete, perform an action
    public virtual void FightComplete() { 
        CancelInvoke(); //stop the invoke repeating
    }

    //count the number of enemies left alive
    //when game objects are destroyed they are set to null
    private void CountEnemies(){
        int numDeadEnemies = 0;
        
        //remove enemies from the list
        foreach(Enemy e in enemies){
            if(e == null){
                numDeadEnemies++;
            }
        }

        //check the number of enemies left
        if(enemies.Count - numDeadEnemies <= 0){
            FightComplete();    //the fight is complete if there are no enemies left
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        //if the player has entered the fight area
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            foreach(Enemy e in enemies){
                e.isHostile = true;
            }

            //check how many enemies are left every 2 seconds
            InvokeRepeating("CountEnemies", 0f, 2f);
        }
    }
}
