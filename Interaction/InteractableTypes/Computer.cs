using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Computer : MonoBehaviour, IObject
{
    public Animator computerUI; //reference to the Computer UI
    public KeyboardController keyboard;

    [SerializeField] private int levelRequired; //the level required to decode the pass code
    [SerializeField] private string passCode;   //the pass code for this computer
    [SerializeField] private TMP_InputField inputField; //the input field to watch
    [SerializeField] private TextMeshProUGUI passCodeLabel; //the label that displays the code for the computer

    public string Instructions => "Press E to Use Computer";

    public bool ShowInstructions => !computerUI.GetBool("computing");   //show the instructions if the player is not in the computer UI (computing = false)

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public bool ShakeCameraOnInteract => !computerUI.GetBool("computing");

    private bool canInteract = true;    //used to disable interactions when typing
    private string codedChars = "!@#*&10";  //the coded characters

    void Start()
    {
        StartCoroutine(RefreshCodedCharacters());
    }

    void Update()
    {
        inputField.enabled = keyboard.KeyboardAvailable;    //can only type if you have a keyboard

        //stop the coroutine if the player has met the required level
        if (keyboard.level >= levelRequired)
        {
            StopCoroutine(RefreshCodedCharacters());
            passCodeLabel.text = passCode;
        }

        //if the interaction with the computer is ending
        if (computerUI.GetCurrentAnimatorStateInfo(0).IsName("hide"))
        {
            //the player is not interactong with the computer
            Controller.interacting = false;
        }
    }

    //enable interactions with the computer
    public void EnableInteract()
    {
        canInteract = true;
    }

    //disable interactions with the computer
    public void DisableInteract()
    {
        canInteract = false;
    }

    //check the pass code
    public void CheckPassCode()
    {
        //if the passcode is correct, execute the function
        if (inputField.text == passCode)
        {
            PassCodeCorrect();
        }
    }

    //action to complete if the interaction is complete
    public virtual void PassCodeCorrect()
    {
        return;
    }

    //interaction
    public void Do()
    {
        if (!canInteract) return;

        //if computing = true (in the computer UI), set it to false and vice versa
        computerUI.SetBool("computing", !computerUI.GetBool("computing"));

        //the player is interacting with the computer
        Controller.interacting = true;
    }

    //coroutine to repeatedly create a code above the computer
    private IEnumerator RefreshCodedCharacters()
    {
        while(true){
            string codedPassCode = "";

            //create a random code
            for (int i = 0; i < Random.Range(4, 8); i++)
            {
                codedPassCode += codedChars[Random.Range(0, codedChars.Length - 1)];
            }

            passCodeLabel.text = codedPassCode; //set the text

            yield return new WaitForSeconds(0.7f);  //wait then repeat
        }
    }
}
