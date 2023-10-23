using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamageable : Damageable
{
    public TextMeshProUGUI healthBarText;   //the text field that displays the health
    [SerializeField] private float iFrameTime;  //the amount of time the player is invulnerable for after taking damage
    private SpriteRenderer playerRenderer;  //reference to the player's sprite renderer
    private bool invulnerable = false;   //is the player invulnerable

    private void Start(){
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();

        RefreshHealthBar();
    }

    //when the player takes damage
    public override void TakeDamage(int damage)
    {
        //if can take damage
        if(!invulnerable){
            health -= damage;   //take damage
            RefreshHealthBar();
            StartCoroutine(Invulnerable());
        }

        if(health <= 0){
            //reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void RefreshHealthBar(){
        healthBarText.text = "";    //remove the text

        string healthDisplay = "";  //the text to represent the players health

        for(int i = 0; i < health; i++){
            healthDisplay += "/";
        }

        healthBarText.text = healthDisplay;
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
