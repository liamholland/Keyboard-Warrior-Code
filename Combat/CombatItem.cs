using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCombatItem", menuName = "Combat Item")]
public class CombatItem : ScriptableObject
{
    public Sprite image;
    public CombatType combatType;
    public bool isWeapon;
    public float damage;
    public float defense;

    public enum CombatType
    {
        Weapon_Staff,
        Weapon_Sword,
        Weapon_Knife,

        Armour_Helmet,
        Armour_ChestPlate,
        Armour_Pants,
        Armour_Boots
    }
}
