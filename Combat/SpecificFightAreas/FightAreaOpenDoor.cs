using UnityEngine;

public class FightAreaOpenDoor : FightArea
{
    [SerializeField] private OneTimeOpenDoor doorToOpen;    //the door to open when this fight is over

    public override void FightComplete()
    {
        base.FightComplete();   //cancels the invoking repeating function
        doorToOpen.Open();  //open the door
    }
}
