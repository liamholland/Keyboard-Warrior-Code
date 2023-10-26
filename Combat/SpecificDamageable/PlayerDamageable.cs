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


    private void Start(){
        playerAnimator = gameObject.GetComponent<Animator>();
    
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();

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

            playerAnimator.SetBool("dead", true);
        }
    }

    /// <summary>
    /// Reload the current scene with player context
    /// </summary>
    public void RetryScene(){
        Controller player = gameObject.GetComponent<Controller>();
        KeyboardController keyboard = player.keyboardController;
        
        //set the context
        PlayerContext context = (PlayerContext)ScriptableObject.CreateInstance("PlayerContext");
        context.canDash = player.canDash;
        context.airControl = player.airControl;
        context.available = keyboard.KeyboardAvailable;
        context.longCableUnlocked = keyboard.longCableUnlocked;
        context.level = keyboard.Level;

        Controller.context = context;

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
