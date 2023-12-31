using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script applied to doors that will be opened once and never closed
public class OneTimeOpenDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Animator doorAnimator; //the animator for the door
    [SerializeField] private bool hideOnOpen = true;   //set the door to inactive upon opening
    [SerializeField] private AudioSource openSound; //sound that plays when the door opens

    public bool IsOpen => doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open");

    void Update(){
        //hide the door if it has completed its open animation
        if(IsOpen && hideOnOpen) gameObject.SetActive(false);
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

        openSound.Play();

        //add it to the player context
        PlayerContext.AddDoorToContext(gameObject);
    }
}
