using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext : ScriptableObject
{
    [Header("-- Player --")]
    public bool airControl;
    public bool canDash;

    [Header("-- Keyboard --")]
    public bool available;
    public bool longCableUnlocked;
    public int level;

    [Header("-- Position --")]
    public Vector2 position;
}
