using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Continue";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    [SerializeField] private string levelToLoad;    //name of the level to load

    public void Do()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
