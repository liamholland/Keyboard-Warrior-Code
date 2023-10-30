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
        
        PlayerContext.spawnPosition = atPosition;
        
        Controller.context = PlayerContext.GenerateNewContext(player, keyboard);   //set the player context

        Controller.context.sceneName = levelToLoad;

        //load the scene
        StartCoroutine(player.LoadSceneAnimation(levelToLoad));
    }
}
