using UnityEngine;

public class CarryItem : IObject
{
    private bool isBeingCarried = false;

    public GameObject player;
    public string instructions;

    public override string Instructions 
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

    public override void Do()
    {
        isBeingCarried = !isBeingCarried;
    }

    private void FixedUpdate()
    {
        if (isBeingCarried)
        {
            transform.position = player.transform.position;
        }
    }
}
