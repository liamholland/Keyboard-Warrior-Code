using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : Menu
{
    public bool isShowing;
    public CanvasGroup chestUI;
    public GameObject itemSlot;
    public GameObject combatSlot;
    public GameObject itemDisplay;

    public Inventory inventory;
    public CharachterScreen cs;

    public List<Item> items = new List<Item>();
    public List<CombatItem> combatItems = new List<CombatItem>();

    private List<GameObject> itemsInChest = new List<GameObject>();
    private GameObject newItemSlot;

    public override bool isOpen => isShowing;

    public override void Close()
    {
        chestUI.blocksRaycasts = false;
        chestUI.alpha = 0;
        foreach(GameObject g in itemsInChest)
        {
            Destroy(g);
        }
        isShowing = false;
    }

    public override void CloseOthers()
    {
        foreach (Menu m in allMenus)
        {
            if (m != this)
            {
                if (m.isOpen)
                    m.Close();
            }
        }
    }

    public override void Open()
    {
        CloseOthers();
        foreach (Item i in items)
        {
            if(items.Count > 0)
            {
                newItemSlot = Instantiate(itemSlot, itemDisplay.transform);
                ItemContainer ic = newItemSlot.GetComponent<ItemContainer>();
                ic.item = i;
                ic.inChest = true;
                ic.inventory = inventory;
                ic.chest = this;
                itemsInChest.Add(newItemSlot);
            }
        }

        foreach(CombatItem c in combatItems)
        {
            if(combatItems.Count > 0)
            {
                newItemSlot = Instantiate(combatSlot, itemDisplay.transform);
                CombatContainer cc = newItemSlot.GetComponent<CombatContainer>();
                cc.combatItem = c;
                cc.inChest = true;
                cc.cs = cs;
                cc.chest = this;
                itemsInChest.Add(newItemSlot);
            }
        }

        chestUI.alpha = 1;
        chestUI.blocksRaycasts = true;
        isShowing = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isShowing)
            Close();
    }
}
