using UnityEngine;

public class BossDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Animator doorAnimator; //reference to the door animator

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
}
