public class LevelOneComputer : Computer
{
    public OneTimeOpenDoor doorToOpen;    //the door this computer opens

    public override void PassCodeCorrect()
    {
        doorToOpen.Open();
    }
}
