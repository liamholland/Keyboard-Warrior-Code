using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Animator doorAnimator; //reference to the door animator
    [SerializeField] private Enemy boss;    //reference to the boss

    public bool IsOpen => doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open");

    void Start(){
        doorAnimator.SetBool("Unlocked", true);
    }

    public void Close()
    {
        doorAnimator.SetBool("Unlocked", false);
    }

    public void Open()
    {
        doorAnimator.SetBool("Unlocked", true);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            Camera.main.orthographicSize = 10f;
            Close();
            boss.isHostile = true;
        }
    }
}
