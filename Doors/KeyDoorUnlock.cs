using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorUnlock : Damageable
{
    public KeyboardController keyboard; //reference to the keyboard

    [SerializeField] private Key doorKey;
    [SerializeField] private OneTimeOpenDoor doorToOpen;

    public override void TakeDamage(int damage)
    {
        //check if the keyboard has the key
        foreach(Key k in keyboard.keys){
            //if the keynames match, unlock the door
            if(k.keyName == doorKey.keyName){
                doorToOpen.Open();
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
