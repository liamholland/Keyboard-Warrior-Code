using UnityEngine;

public class CarryItem : MonoBehaviour, IObject
{
    private bool isBeingCarried = false;

    public GameObject player;
    public string instructions;

    public string Instructions 
    { 
        get 
        {
            if (!isBeingCarried) 
            {
                return "Press E to Carry";
            }
            else
            {
                return "Press E to Drop";
            }
        } 
    }

    public bool ShowInstructions => true;

    public void Do()
    {
        isBeingCarried = !isBeingCarried;
    }

    private void Update()
    {
        if (isBeingCarried)
        {
            transform.position = player.transform.position;
        }
    }
}
