using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Damageable : EnemyDamageable
{
    public GameObject longCableUnlock;  //the unlock for the long cable
    public BossDoor bossRoomDoor;  //the boss room door
    public Animator animator;
    public Rigidbody2D bossRigid;   //the boss rigidbody
    public Enemy boss;

    public override void TakeDamage(int damage)
    {
        if(health <= 0) return; //dont do anything if already dead

        health -= damage;   //reduce health

        //check if the boss is dead now
        if(health <= 0){
            StartCoroutine(Die());
        }
        else{
            StartCoroutine(FlashOnDamage());
        }
    }

    private IEnumerator Die(){
        boss.EnemyTarget = transform.position;

        animator.SetBool("dead", true);
        animator.applyRootMotion = false;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Escape"));

        bossRigid.gravityScale = 0f;

        yield return new WaitUntil(() => transform.position.y > 30f);

        Instantiate(longCableUnlock, transform.position, Quaternion.identity);

        bossRoomDoor.Open();    //open the boss room door

        gameObject.SetActive(false);
    }
}
