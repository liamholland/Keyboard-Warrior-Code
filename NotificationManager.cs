using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{

    [Header("-- Full Notification --")]
    [SerializeField] private Animator fullNotificationAnimator;
    [SerializeField] private TextMeshProUGUI titleTextFullNotif;
    [SerializeField] private TextMeshProUGUI notificationTextFullNotif;
    [SerializeField] private Image notficationImage;

    [Header("-- Pop-Up Notification")]
    [SerializeField] private Animator popUpNotificationAnimator;
    [SerializeField] private TextMeshProUGUI titleTextPopUp;
    [SerializeField] private TextMeshProUGUI notificationTextPopUp;

    private static bool fullNotifActive = false;  //is there a full notification active
    private static bool popUpNotifActive = false;  //is there a pop up active
    private static Queue<Notification> popUpQueue = new Queue<Notification>(); //queue for popups

    //public getters
    public static bool FullNotifActive => fullNotifActive;
    public static NotificationManager Manager => manager;

    //singleton instance
    private static NotificationManager manager;
    
    //delay object
    private static WaitForSeconds fourSecondDelay = new WaitForSeconds(4f);

    private void Awake(){
        //update the notification manager when the scene starts
        manager = this;
    }

    /// <summary>
    /// Displays a full screen notification
    /// </summary>
    /// <param name="notification">The notification object to display</param>
    public void ShowFullNotification(Notification notification){
        if(fullNotifActive) return; //do nothing if one is already showing
        
        fullNotifActive = true;

        //set the ui values
        titleTextFullNotif.text = notification.notificationTitle;
        notificationTextFullNotif.text = notification.notificationText;
        notficationImage.sprite = notification.notificationImage;

        //diplay the notification
        fullNotificationAnimator.SetBool("computing", true);
    }

    /// <summary>
    /// Hide the notification UI
    /// </summary>
    public void HideFullNotification(){
        fullNotificationAnimator.SetBool("computing", false);

        fullNotifActive = false;
    }

    /// <summary>
    /// Show a pop up notification
    /// </summary>
    /// <param name="notification">The notification object to display (Images will be ignored)</param>
    public void ShowPopUpNotification(Notification notification){
        if(popUpNotifActive){
            popUpQueue.Enqueue(notification);
            return;
        }
        
        popUpNotifActive = true;

        //set the UI values
        titleTextPopUp.text = notification.notificationTitle;
        notificationTextPopUp.text = notification.notificationText;

        //start animation
        StartCoroutine(AnimatePopUp());
    }

    //coroutine to animate the pop up
    private IEnumerator AnimatePopUp(){
        if(fullNotifActive){
            yield return new WaitUntil(() => fullNotificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("hiding"));
        }

        popUpNotificationAnimator.SetBool("notifying", true);

        yield return fourSecondDelay;

        popUpNotificationAnimator.SetBool("notifying", false);

        yield return new WaitUntil(() => popUpNotificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("hiding"));
        
        popUpNotifActive = false;

        //if there is another notification in the queue, then show that one next
        if(popUpQueue.Count > 0){
            ShowPopUpNotification(popUpQueue.Dequeue());
        }
    }
}
