using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObject : MonoBehaviour, IObject
{
    [SerializeField] private bool useDefaultInteract = true;    //should the tutorial use the default interact
    [SerializeField] private string keycodeToUseInstead;    //the keycode to use if not
    [SerializeField] private string toDoWhat;   //end of the intructions
    [SerializeField] private bool available = true;   //is the tutorial available

    public string Instructions => "Press " + (useDefaultInteract ? "E" : keycodeToUseInstead) + toDoWhat;

    public bool ShowInstructions => true;

    public bool UseDefaultInteractButton => useDefaultInteract;

    public string CustomKeyCode => keycodeToUseInstead;

    public bool ShakeCameraOnInteract => false;
    public bool IsAvailable{
        get => available;
        set{
            available = value;
            tutorialCollider.enabled = value;   //disable or enable the collider on the object
        }
    }

    private CircleCollider2D tutorialCollider;  //reference to the tutorial's collider

    private void Start(){
        //get a reference to the collider
        tutorialCollider = GetComponent<CircleCollider2D>();

        IsAvailable = available;    //set the public accessor with whatever was set in the inspector
    }

    public void Do()
    {
        gameObject.SetActive(false);    //just make the object inactive
    }
}
