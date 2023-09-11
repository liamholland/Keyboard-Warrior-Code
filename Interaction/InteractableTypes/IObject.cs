using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObject : MonoBehaviour
{
    public abstract string Instructions { get; }
    public abstract void Do();
}
