using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Learn More Programming";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public KeyboardController keyboardController;   //reference to the keyboard controller
    public OneTimeOpenDoor doorToOpen;    //reference to a door the level up might open

    public void Do()
    {
        //level up the player
        keyboardController.Level++;

        //if there is a door to open
        if(doorToOpen != null){
            doorToOpen.Open();  //open the door
        }

        //add it to the player context
        PlayerContext.AddLevelUpToContext(gameObject);

        //remove the interactable
        gameObject.SetActive(false);
    }
}
