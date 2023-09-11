using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatContainer : MonoBehaviour, IPointerClickHandler
{
    public CombatItem combatItem;
    public CharachterScreen cs;
    
    [HideInInspector] public bool isEquipped;
    [HideInInspector] public bool inChest;
    [HideInInspector] public ChestUI chest;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inChest)
        {
            inChest = false;
            cs.PickUp(combatItem);
            chest.combatItems.Remove(combatItem);
            Destroy(gameObject);
        }
        else
            cs.Equip(gameObject);
    }
}
