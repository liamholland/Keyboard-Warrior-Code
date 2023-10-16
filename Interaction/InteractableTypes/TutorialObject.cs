using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : MonoBehaviour, IObject
{
    [SerializeField] private bool useDefaultInteract = true;    //should the tutorial use the default interact
    [SerializeField] private string keycodeToUseInstead;    //the keycode to use if not
    [SerializeField] private string toDoWhat;   //end of the intructions

    public string Instructions => "Press " + (useDefaultInteract ? "E" : keycodeToUseInstead) + toDoWhat;

    public bool ShowInstructions => true;

    public bool UseDefaultInteractButton => useDefaultInteract;

    public string CustomKeyCode => keycodeToUseInstead;

    public bool ShakeCameraOnInteract => false;

    public void Do()
    {
        gameObject.SetActive(false);    //just make the object inactive
    }
}
