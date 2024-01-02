public class HealComputer : Computer
{
    public PlayerDamageable playerDamageable;   //reference to the player damageable

    public override void PassCodeCorrect()
    {
        playerDamageable.HealPlayer();
    }
}
