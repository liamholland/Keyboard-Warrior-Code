using UnityEngine;
public class CollectionItem : IObject
{
    public Item item;
    public CombatItem combatItem;
    public Inventory inventory;
    public CharachterScreen cs;

    public override string Instructions { get => "Press E to Collect"; }

    public override void Do()
    {
        if (item != null)
            inventory.PickUp(item);
        else if (combatItem != null)
            cs.PickUp(combatItem);

        Destroy(gameObject);
    }
}
