using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float baseHealth;
    public float baseDamage;
    public float baseDefense;
    public GameObject[] buffs;

    public CharachterScreen cs;

    public float CalculateDamage()
    {
        float dmg = baseDamage;

        foreach(GameObject g in cs.equippedItems)
        {
            CombatContainer c = g.GetComponent<CombatContainer>();

            if (c.combatItem.isWeapon)
                dmg += c.combatItem.damage;
        }

        return dmg;
    }

    public float CalculateDefense()
    {
        float dfns = baseDefense;

        foreach(GameObject g in cs.equippedItems)
        {
            CombatContainer c = g.GetComponent<CombatContainer>();

            if (!c.combatItem.isWeapon)
                dfns += c.combatItem.defense;
        }

        return dfns;
    }

    public void ApplyBuffs()
    {
        foreach(GameObject g in buffs)
        {
            //apply buff or debuff to enemy
        }
    }
}
