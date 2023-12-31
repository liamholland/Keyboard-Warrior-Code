using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StartBoss1 : MonoBehaviour, IObject
{
    public string Instructions => "Press E to Fight";

    public bool ShowInstructions => true;

    public bool ShakeCameraOnInteract => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public Camera mainCamera;
    public GameObject player;

    [SerializeField] private Animator bossAnimator;
    [SerializeField] private Animator bossHealthBarAnimator;
    [SerializeField] private Rigidbody2D bossRigid;
    [SerializeField] private LineRenderer cablesHoldingBoss;
    [SerializeField] private GameObject[] grabbableCableObjects;
    [SerializeField] private BossDoor bossDoor;
    [SerializeField] private Enemy boss;
    [SerializeField] private AudioSource bossMusic;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update(){
        float distToPlayer = 7f - Vector2.Distance(transform.position, player.transform.position);

        mainCamera.orthographicSize = 5f + (distToPlayer > 0f ? distToPlayer : 0f);
    }

    public void Do()
    {
        bossDoor.Close();

        cablesHoldingBoss.enabled = false;

        foreach(GameObject g in grabbableCableObjects){
            g.SetActive(true);
        }

        bossAnimator.SetBool("fightStart", true);

        bossRigid.gravityScale = 1f;

        boss.isHostile = true;

        bossHealthBarAnimator.SetBool("inConversation", true);

        bossMusic.Play();

        gameObject.SetActive(false);
    }
}
