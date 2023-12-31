using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamageable : Damageable
{
    public TextMeshProUGUI healthBarText;   //the text field that displays the health
    [SerializeField] private float iFrameTime;  //the amount of time the player is invulnerable for after taking damage

    private Animator playerAnimator;   //reference to the player animator
    private SpriteRenderer playerRenderer;  //reference to the player's sprite renderer
    private bool invulnerable = false;   //is the player invulnerable
    private int fullHealth; //the health the player has at max health
    private Color playerColour;  //the colour of the player

    //delay object
    private static WaitForSeconds oneTenthSecondDelay = new WaitForSeconds(0.1f);

    private void Start(){
        playerAnimator = gameObject.GetComponent<Animator>();
    
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();

        playerColour = playerRenderer.color;    //set the player's colour

        fullHealth = health;    //record the player's max health

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
            Controller.isInteracting = true;

            //add the death to the context
            PlayerContext.numDeaths++;

            playerAnimator.SetBool("dead", true);
        }
    }

    /// <summary>
    /// Reload the current scene with player context
    /// </summary>
    public void RetryScene(){
        Controller player = gameObject.GetComponent<Controller>();
        KeyboardController keyboard = player.keyboardController;

        Controller.context = PlayerContext.GenerateNewContext(player, keyboard);

        //reload the current scene
        StartCoroutine(player.LoadSceneAnimation(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// Load the main menu
    /// </summary>
    public void LoadMainMenu(){
        Controller player = gameObject.GetComponent<Controller>();
        StartCoroutine(player.LoadSceneAnimation("MainMenu"));
    }

    private void RefreshHealthBar(){
        healthBarText.text = "";    //remove the text

        string healthDisplay = "";  //the text to represent the players health

        for(int i = 0; i < health; i++){
            healthDisplay += "/";
        }

        healthBarText.text = healthDisplay;
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void HealPlayer(){
        health = fullHealth;    //player is back to full health

        RefreshHealthBar(); //refresh the health bar
    }

    //apply a stun affect to the player
    private IEnumerator Invulnerable(){
        invulnerable = true;

        float elapsedTime = 0f;

        //flash the player
        while(elapsedTime <= iFrameTime){

            playerRenderer.color = new Color(0, 0, 0, 1);

            yield return oneTenthSecondDelay;

            playerRenderer.color = playerColour;

            yield return oneTenthSecondDelay;

            elapsedTime += Time.fixedDeltaTime;
        }

        playerRenderer.color = playerColour;

        invulnerable = false;
    }
}
