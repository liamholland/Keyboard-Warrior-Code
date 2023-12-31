using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardUnlock : MonoBehaviour, IObject
{
    public KeyboardController playerKeyboard;

    [SerializeField] private OneTimeOpenDoor doorToOpen;    //the keyboard unlock needs to open a door
    [SerializeField] private Conversation[] conversationToMakeAvailable;  //npcs will comment on the keyboard 
    [SerializeField] private Conversation[] conversationsToMakeUnavailable; //unlock will make some conversations unavailable
    [SerializeField] private Notification notificationToShow;   //the notification displayed on pickup

    public string Instructions { get => "Press E to Become a Keyboard Warrior"; }

    public bool ShowInstructions => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public bool ShakeCameraOnInteract => true;

    //enable the player's keyboard
    public void Do()
    {
        playerKeyboard.KeyboardAvailable = true;
        
        //make conversations available
        foreach(Conversation c in conversationToMakeAvailable){
            c.IsAvailable = true;
        }

        foreach(Conversation c in conversationsToMakeUnavailable){
            c.IsAvailable = false;
        }

        doorToOpen.Open();
        NotificationManager.Manager.ShowFullNotification(notificationToShow);
        gameObject.SetActive(false);
    }
}
