using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            PlayerContext.spawnPosition = gameObject.transform.position;
            gameObject.SetActive(false);
        }
    }
}
