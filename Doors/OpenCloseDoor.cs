using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script applied to doors that can open and close at different points in the game
public class OpenCloseDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Animator doorAnimator; //the animator of the door
    [SerializeField] private bool openAtStart;  //the state of the door at the start of the scene
    public bool IsOpen => doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open");

    void Start(){
        doorAnimator.SetBool("Unlocked", openAtStart);
    }

    public void Close()
    {
        doorAnimator.SetBool("Unlocked", false);
    }

    public void Open()
    {
        doorAnimator.SetBool("Unlocked", true);
    }
}
