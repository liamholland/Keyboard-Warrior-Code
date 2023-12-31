using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Collect";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    [SerializeField] private Notification collectibleNotification;

    public void Do()
    {
        PlayerContext.numCollectiblesFound++;   //increment the number of collectibles found

        PlayerContext.AddCollectibleToContext(gameObject);  //add the collectible to the context

        NotificationManager.Manager.ShowPopUpNotification(collectibleNotification);

        gameObject.SetActive(false);    //destroy the collectible
    }
}
