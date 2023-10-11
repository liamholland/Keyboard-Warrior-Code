using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardUnlock : MonoBehaviour, IObject
{
    public KeyboardController playerKeyboard;

    public string Instructions { get => "Become a Keyboard Warrior"; }

    //enable the player's keyboard
    public void Do()
    {
        playerKeyboard.KeyboardAvailable = true;
        Destroy(gameObject);
    }
}
