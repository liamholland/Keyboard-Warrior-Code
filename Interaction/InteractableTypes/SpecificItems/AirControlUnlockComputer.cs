public class AirControlUnlockComputer : Computer
{
    public Controller playerController; //reference to the player controller
    public TutorialObject airControlTutorial;   //reference to the air control tutorial

    public override void PassCodeCorrect()
    {
        //the player can use air control when they use this computer
        playerController.airControl = true;

        airControlTutorial.IsAvailable = true;  //enable the tutorial
    }
}
