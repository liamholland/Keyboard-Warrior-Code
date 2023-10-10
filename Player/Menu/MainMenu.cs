using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public CanvasGroup mainMenu;
    public Inventory inventory;

    private bool isShowing;

    public override bool isOpen => isShowing;

    public override void Close()
    {
        mainMenu.alpha = 0;
        mainMenu.blocksRaycasts = false;
        isShowing = false;
    }

    public override void CloseOthers()
    {
        foreach (Menu m in allMenus)
        {
            if (m != this)
            {
                if(m.isOpen)
                    m.Close();
            }
        }
    }

    public override void Open()
    {
        CloseOthers();
        isShowing = true;
        mainMenu.blocksRaycasts = true;
        mainMenu.alpha = 1;
    }

    void Start()
    {
        allMenus.Add(this);
        isShowing = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (!isShowing)
                Open();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void InventoryButton()
    {
        Close();
        inventory.Open();
    }

    public void CharachterScreenButton()
    {
        Close();
    }
}
