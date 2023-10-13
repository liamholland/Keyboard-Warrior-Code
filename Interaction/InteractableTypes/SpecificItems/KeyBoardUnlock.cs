using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardUnlock : MonoBehaviour, IObject
{
    public KeyboardController playerKeyboard;

    [SerializeField] private OneTimeOpenDoor doorToOpen;    //the keyboard unlock needs to open a door

    public string Instructions { get => "Become a Keyboard Warrior"; }

    //enable the player's keyboard
    public void Do()
    {
        playerKeyboard.KeyboardAvailable = true;
        doorToOpen.Open();
        gameObject.SetActive(false);
    }
}
