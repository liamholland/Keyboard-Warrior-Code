using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Move Scene";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    [SerializeField] private string levelToLoad;    //name of the level to load
    [SerializeField] private Vector2 spawnPointInOtherScene;    //the point this scene loader moves the player to
    private GameObject player;  //the player

    private void Start(){
        player = GameObject.Find("Player"); //find the player instance
    }

    public void Do()
    {
        //load the scene
        SceneManager.LoadScene(levelToLoad);

        //move the player
        // player.transform.position = spawnPointInOtherScene;
    }
}
