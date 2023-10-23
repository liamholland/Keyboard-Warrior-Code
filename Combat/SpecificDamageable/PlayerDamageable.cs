using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamageable : Damageable
{
    [SerializeField] private float iFrameTime;  //the amount of time the player is invulnerable for after taking damage
    private SpriteRenderer playerRenderer;  //reference to the player's sprite renderer
    private bool invulnerable = false;   //is the player invulnerable

    private void Start(){
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    //when the player takes damage
    public override void TakeDamage(int damage)
    {
        //if can take damage
        if(!invulnerable){
            health -= damage;   //take damage
        }

        if(health <= 0){
            //reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else{
            if(!invulnerable){
                Debug.Log(health);
                StartCoroutine(Invulnerable());
            }
        }
    }

    //apply a stun affect to the player
    private IEnumerator Invulnerable(){
        invulnerable = true;

        float elapsedTime = 0f;

        Color playerColor = playerRenderer.color;

        //flash the player
        while(elapsedTime <= iFrameTime){

            playerRenderer.color = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(0.1f);

            playerRenderer.color = playerColor;

            yield return new WaitForSeconds(0.1f);

            elapsedTime += Time.fixedDeltaTime;
        }

        playerRenderer.color = playerColor;

        invulnerable = false;
    }
}
