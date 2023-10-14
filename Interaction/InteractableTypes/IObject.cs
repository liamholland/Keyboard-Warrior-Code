
using UnityEngine;

public interface IObject
{
    public string Instructions { get; }
    public bool ShowInstructions { get; }
    public void Do();
}
