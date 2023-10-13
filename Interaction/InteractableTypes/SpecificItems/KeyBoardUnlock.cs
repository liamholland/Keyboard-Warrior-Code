using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardUnlock : MonoBehaviour, IObject
{
    public KeyboardController playerKeyboard;

    [SerializeField] private OneTimeOpenDoor doorToOpen;    //the keyboard unlock needs to open a door
    [SerializeField] private Conversation[] conversationToMakeAvailable;  //npcs will comment on the keyboard 

    public string Instructions { get => "Become a Keyboard Warrior"; }

    //enable the player's keyboard
    public void Do()
    {
        playerKeyboard.KeyboardAvailable = true;
        
        //make conversations available
        foreach(Conversation c in conversationToMakeAvailable){
            c.isAvailable = true;
        }

        doorToOpen.Open();
        gameObject.SetActive(false);
    }
}
