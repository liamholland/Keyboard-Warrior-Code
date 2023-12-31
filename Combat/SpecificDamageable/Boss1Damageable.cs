using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Damageable : EnemyDamageable
{
    public GameObject longCableUnlock;  //the unlock for the long cable
    public GameObject keyUnlock; //the unlock for the red key
    public BossDoor bossRoomDoor;  //the boss room door
    public Animator animator;
    public Rigidbody2D bossRigid;   //the boss rigidbody
    public Enemy boss;
    [SerializeField] private Notification deathNotification;    //the pop up displayed when the boss dies
    [SerializeField] private AudioSource bossMusic; //the boss music

    [Header("-- Health Bar --")]
    public Animator bossHealthBarAnimator;  //the animator of the health bar
    public Image healthBar;

    private int maxHealth;  //the max health of the boss
    private float originalHealthBarX;

    private void Start(){
        maxHealth = health; //record the max health
        originalHealthBarX = healthBar.rectTransform.sizeDelta.x;
    }

    public override void TakeDamage(int damage)
    {
        
        if(health <= 0) return; //dont do anything if already dead

        health -= damage;   //reduce health

        takeDamageSound.Play();

        //update the health bar
        healthBar.rectTransform.sizeDelta = new Vector2(originalHealthBarX * ((float)health / maxHealth), healthBar.rectTransform.sizeDelta.y);

        //check if the boss is dead now
        if(health <= 0){
            StartCoroutine(Die());
        }
        else{
            StartCoroutine(FlashOnDamage());
        }
    }

    private IEnumerator Die(){
        bossHealthBarAnimator.SetBool("inConversation", false); //im using the dialogue box animator

        bossMusic.Stop();

        boss.EnemyTarget = transform.position;

        animator.SetBool("dead", true);
        animator.applyRootMotion = false;

        deathSounds[0].Play();

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Escape"));

        bossRigid.gravityScale = 0f;

        yield return new WaitUntil(() => transform.position.y > 30f);

        Instantiate(longCableUnlock, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
        Instantiate(keyUnlock, new Vector2(transform.position.x - 1, transform.position.y), Quaternion.identity);

        NotificationManager.Manager.ShowPopUpNotification(deathNotification);

        bossRoomDoor.Open();    //open the boss room door

        gameObject.SetActive(false);
    }
}
