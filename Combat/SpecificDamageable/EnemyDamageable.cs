using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable
{
    public Enemy enemyController;   //reference to the enemy controller
    public SpriteRenderer enemyRenderer;    //renderer of the enemy
    public Rigidbody2D enemyRigid;  //rigid body of the enemy
    [Range(0.05f, 10f)] [SerializeField] private float deathGravityScale;
    [Range(0f, 0.5f)] [SerializeField] private float damageFlashTime;   //amount of time the enemy flashes for damage
    [SerializeField] private Color damageFlashColor;  //the color that the enemy flashes when it takes damage
    [SerializeField] private Conversation[] conversationsToMakeAvailableOnDeath;    //the conversations made available upon defeat of the enemy
    [SerializeField] private AudioSource[] deathSounds; //selection of sounds to play on death
    [SerializeField] private AudioSource takeDamageSound;   //sound made when taking damage

    //what the enemy does when it takes damage
    public override void TakeDamage(int damage)
    {
        health -= damage;   //reduce the enemies health

        enemyController.isHostile = true;   //make the enemy hostile
        
        takeDamageSound.Play();

        //make the enemy flash to indicate damage was taken
        StartCoroutine(FlashOnDamage());
    }

    public IEnumerator FlashOnDamage()
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
            PlayerContext.enemiesKilled++;

            //play a random death sound
            deathSounds[Random.Range(0, deathSounds.Length)].Play();

            //make all conversations available
            foreach(Conversation c in conversationsToMakeAvailableOnDeath){
                c.IsAvailable = true;
            }

            enemyController.enabled = false;

            enemyRigid.mass = 1f;
            enemyRigid.gravityScale = deathGravityScale;
            enemyRigid.constraints = RigidbodyConstraints2D.None;
            transform.Rotate(0f, 0f, Random.Range(15f, 100f));

            Destroy(gameObject, 3f);
        }
    }
}
