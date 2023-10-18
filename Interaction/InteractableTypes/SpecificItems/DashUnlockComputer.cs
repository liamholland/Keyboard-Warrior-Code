using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashUnlockComputer : Computer
{
    public Controller playerController; //reference to the player controller
    public OneTimeOpenDoor doorToOpen;  //this computer has a door to open

    //the player can dash when they get the passcode correct
    public override void PassCodeCorrect()
    {
        if(!doorToOpen.IsOpen){
            doorToOpen.Open();  //open the door
        }

        playerController.canDash = true;    //the player can dash now
    }
}
