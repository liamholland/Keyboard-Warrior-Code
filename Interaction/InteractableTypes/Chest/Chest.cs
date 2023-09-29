using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IObject
{
    public ChestUI chestUI;

    public string Instructions { get => "Press E to Open"; }

    public void Do()
    {
        if(!chestUI.isShowing)
            chestUI.Open();
    }
}
