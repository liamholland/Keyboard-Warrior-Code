using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUnlcok : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Add a Key to the Keyboard";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    [SerializeField] private Notification unlockNotification;   //the notification to show on pick up
    [SerializeField] private Key key;
    private KeyboardController keyboard;

    private void Start(){
        //get a reference to the keyboard object
        keyboard = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<KeyboardController>();
    }

    public void Do()
    {
        keyboard.keys.Add(key); //add the key to the keyboard

        NotificationManager.Manager.ShowFullNotification(unlockNotification);

        gameObject.SetActive(false);    //remove the game object
    }
}
