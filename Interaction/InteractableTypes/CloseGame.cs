using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseGame : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Continue";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public void Do()
    {
        SceneManager.LoadScene("Level2");
    }
}
