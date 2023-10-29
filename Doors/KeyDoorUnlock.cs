using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorUnlock : MonoBehaviour
{
    [SerializeField] private Key doorKey;
    [SerializeField] private OneTimeOpenDoor doorToOpen;

    private void OnTriggerEnter2D(Collider2D other){
        KeyboardController keyboard = other.gameObject.GetComponent<KeyboardController>();

        if(keyboard != null){
            foreach(Key key in keyboard.keys){
                if(key.keyName == doorKey.keyName){
                    doorToOpen.Open();
                    break;
                }
            }
        }
    }
}
