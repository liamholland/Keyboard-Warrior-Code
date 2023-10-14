using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, IObject
{
    public Animator computerUI; //reference to the Computer UI

    public string Instructions => "Press E to Use Computer";

    public bool ShowInstructions => !computerUI.GetBool("computing");   //show the instructions if the player is not in the computer UI (computing = false)

    public void Do()
    {
        //if computing = true (in the computer UI), set it to false and vice versa
        computerUI.SetBool("computing", !computerUI.GetBool("computing"));

        //the player is now interacting with something (or not)
        Controller.interacting = computerUI.GetBool("computing");
    }
}
