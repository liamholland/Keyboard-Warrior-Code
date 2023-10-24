using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable
{
    public Enemy enemyController;   //reference to the enemy controller
    public SpriteRenderer enemyRenderer;    //renderer of the enemy
    [Range(0f, 0.5f)] [SerializeField] private float damageFlashTime;   //amount of time the enemy flashes for damage
    [SerializeField] private Color damageFlashColor;  //the color that the enemy flashes when it takes damage
    [SerializeField] private Conversation[] conversationsToMakeAvailableOnDeath;    //the conversations made available upon defeat of the enemy

    //what the enemy does when it takes damage
    public override void TakeDamage(int damage)
    {
        health -= damage;   //reduce the enemies health

        enemyController.isHostile = true;   //make the enemy hostile
        
        //make the enemy flash to indicate damage was taken
        StartCoroutine(FlashOnDamage());
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
            //enemy is dead

            //make all conversations available
            foreach(Conversation c in conversationsToMakeAvailableOnDeath){
                c.IsAvailable = true;
            }

            Destroy(gameObject);
        }
    }
}
