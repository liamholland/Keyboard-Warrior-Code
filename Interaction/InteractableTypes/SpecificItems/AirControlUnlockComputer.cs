using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControlUnlockComputer : Computer
{
    public Controller playerController; //reference to the player controller

    //the player can use air control when they use this computer
    public override void PassCodeCorrect()
    {
        playerController.airControl = true;
    }
}
