using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashUnlockComputer : Computer
{
    public Controller playerController; //reference to the player controller
    public OneTimeOpenDoor[] doorsToOpen;  //this computer has a door multiple doors to open

    //the player can dash when they get the passcode correct
    public override void PassCodeCorrect()
    {
        foreach(OneTimeOpenDoor door in doorsToOpen){
            if(!door.IsOpen){
                door.Open();  //open the door
            }
        }


        playerController.canDash = true;    //the player can dash now
    }
}
