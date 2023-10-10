using UnityEngine;
public class CollectionItem : MonoBehaviour, IObject
{
    public Item item;
    // public CombatItem combatItem;
    public Inventory inventory;
    // public CharachterScreen cs;

    public string Instructions { get => "Press E to Collect"; }

    public void Do()
    {
        if (item != null)
            inventory.PickUp(item);
        // else if (combatItem != null)
        //     cs.PickUp(combatItem);

        Destroy(gameObject);
    }
}
