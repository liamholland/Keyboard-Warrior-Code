using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public ItemType itemType;

    public enum ItemType
    {
        Food,
        Currency,
        Healing,
    }
}
