using UnityEngine;

[CreateAssetMenu(fileName = "newNotification", menuName = "Notification")]
public class Notification : ScriptableObject
{
    public Sprite notificationImage;
    public string notificationTitle;
    public string notificationText;
}
