using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : IObject
{
    public ChestUI chestUI;

    public override string Instructions { get => "Press E to Open"; }

    public override void Do()
    {
        if(!chestUI.isShowing)
            chestUI.Open();
    }
}
