using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Notification gameSavedNotifcation;

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            PlayerContext.spawnPosition = gameObject.transform.position;
            NotificationManager.Manager.ShowPopUpNotification(gameSavedNotifcation);
            gameObject.SetActive(false);
        }
    }
}
