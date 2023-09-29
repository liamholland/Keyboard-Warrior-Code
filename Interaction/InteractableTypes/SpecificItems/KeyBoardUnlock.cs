using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardUnlock : MonoBehaviour, IObject
{
    public GameObject playerKeyboard;

    public string Instructions { get => "Become a Keyboard Warrior"; }

    //enable the player's keyboard
    public void Do()
    {
        playerKeyboard.SetActive(true);
        Destroy(gameObject);
    }
}
