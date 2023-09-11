using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item item;
    public Inventory inventory;

    [HideInInspector] public bool isHighlighted;
    [HideInInspector] public bool inChest;
    [HideInInspector] public ChestUI chest;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inChest)
        {
            inChest = false;
            inventory.PickUp(item);
            chest.items.Remove(item);
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
    }
}
