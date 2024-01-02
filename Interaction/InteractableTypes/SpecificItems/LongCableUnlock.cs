using UnityEngine;

public class LongCableUnlock : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Extend Keyboard Cable";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public KeyboardController keyboardController;   //reference to the keyboard controller
    
    [SerializeField] private Notification unlockNotification;   //the notification to display

    private void Start(){
        keyboardController = GameObject.FindGameObjectWithTag("Keyboard").GetComponent<KeyboardController>();
    }

    public void Do()
    {
        keyboardController.longCableUnlocked = true;    //unlock the long cable

        NotificationManager.Manager.ShowFullNotification(unlockNotification);

        gameObject.SetActive(false);    //remove the unlock from the game
    }
}
