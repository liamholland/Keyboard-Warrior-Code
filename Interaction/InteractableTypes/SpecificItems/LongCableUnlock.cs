using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongCableUnlock : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Extend Keyboard Cable";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public KeyboardController keyboardController;   //reference to the keyboard controller
    public TutorialObject grappleTutorial;  //reference to the grapple tutorial

    public void Do()
    {
        keyboardController.longCableUnlocked = true;    //unlock the long cable

        grappleTutorial.IsAvailable = true; //enable the tutorial

        gameObject.SetActive(false);    //remove the unlock from the game
    }
}
