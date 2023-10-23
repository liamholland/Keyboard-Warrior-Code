using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTargetOnAttack : Attack
{
    public Vector2 teleportLocation;    //the location this attack teleports the victim to

    public override void DoAttack(Collider2D colliderToDamage)
    {
        if(colliderToDamage != null){
            //teleport the target to the location
            colliderToDamage.gameObject.transform.position = teleportLocation;
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(teleportLocation, 1f);
    }
}
