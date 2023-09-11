using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Menu
{
    public CanvasGroup inventory;
    public GameObject itemSection;
    public GameObject itemSlot;
    public MainMenu mainMenu;

    public Image itemImage;
    public Text itemName;
    public Text itemType;
    public Text itemDescription;

    private List<Item> items = new List<Item>();
    private GameObject newItemSlot;
    private bool isShowing;

    [HideInInspector] public List<GameObject> itemsInInventory = new List<GameObject>();

    public override bool isOpen => isShowing;

    private void Start()
    {
        allMenus.Add(this);
        isShowing = false;
    }

    public override void Close()
    {
        inventory.alpha = 0;
        inventory.blocksRaycasts = false;
        ClearInventory();
        isShowing = false;
        mainMenu.Open();
    }

    private void ClearInventory()
    {
        foreach (GameObject g in itemsInInventory)
        {
            Destroy(g);
        }
    }

    public override void Open()
    {
        CloseOthers();
        isShowing = true;
        foreach (Item i in items)
        {
            newItemSlot = Instantiate(itemSlot, itemSection.transform);
            newItemSlot.GetComponent<ItemContainer>().item = i;
            itemsInInventory.Add(newItemSlot);
        }

        inventory.blocksRaycasts = true;
        inventory.alpha = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isShowing)
            Close();

        if (isShowing)
        {
            DisplayItemInfo();
        }
    }

    private void DisplayItemInfo()
    {
        foreach (GameObject g in itemsInInventory)
        {
            if (g != null)
            {
                ItemContainer i = g.GetComponent<ItemContainer>();
                if (i.isHighlighted)
                {
                    itemImage.sprite = i.item.image;
                    itemName.text = i.item.name;
                    itemType.text = i.item.itemType.ToString();
                    //Add description when implemented
                }
            }
        }
    }

    public void PickUp(Item i)
    {
        items.Add(i);
    }

    public override void CloseOthers()
    {
        foreach(Menu m in allMenus)
        {
            if (m != this)
            {
                if (m.isOpen)
                    m.Close();
            }
        }
    }
}
