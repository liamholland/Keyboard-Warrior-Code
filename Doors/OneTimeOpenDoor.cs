using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script applied to doors that will be opened once and never closed
public class OneTimeOpenDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Animator doorAnimator; //the animator for the door

    public bool IsOpen => doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open");

    void Update(){
        //destroy the door if it has completed its open animation
        if(IsOpen) Destroy(gameObject);
    }

    //not used in this case
    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Open()
    {
        //animate the door opening
        doorAnimator.SetBool("Unlocked", true);
    }
}
