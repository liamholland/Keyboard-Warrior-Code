using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Travel " + whereTo;

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    [SerializeField] private string levelToLoad;    //name of the level to load
    [SerializeField] private string whereTo;    //name of the area to show in instructions
    [SerializeField] private Vector2 atPosition;    //the position to send the player to upon load

    public void Do()
    {
        //get a reference to the player and the keyboard
        Controller player = GameObject.Find("Player").GetComponent<Controller>();
        KeyboardController keyboard = player.keyboardController;

        //create a context
        PlayerContext context = (PlayerContext)ScriptableObject.CreateInstance("PlayerContext");
        context.airControl = player.airControl;
        context.canDash = player.canDash;
        context.available = keyboard.KeyboardAvailable;
        context.longCableUnlocked = keyboard.longCableUnlocked;
        context.level = keyboard.Level;
        context.keys = keyboard.keys;
        
        PlayerContext.spawnPosition = atPosition;
        
        Controller.context = context;   //set the player context

        //load the scene
        StartCoroutine(player.LoadSceneAnimation(levelToLoad));
    }
}
