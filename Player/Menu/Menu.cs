using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public abstract void Open();

    public abstract void Close();

    public abstract void CloseOthers();

    public static List<Menu> allMenus = new List<Menu>();

    public abstract bool isOpen { get; }
}
