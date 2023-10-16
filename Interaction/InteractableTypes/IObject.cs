
using UnityEngine;

public interface IObject
{
    public string Instructions { get; } //the instructions displayed on the UI
    public bool ShowInstructions { get; }   //whether the UI should be shown for the interactable
    public bool ShakeCameraOnInteract { get; }  //should the camera shake when interacting
    public bool UseDefaultInteractButton { get; }   //should the interactable use the button set in the input manager
    public string CustomKeyCode { get; }    //the keycode to use instead of the default one
    public void Do();   //the code to execute upon interaction
}
