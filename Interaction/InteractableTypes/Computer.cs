using System.Collections;
using TMPro;
using UnityEngine;

public class Computer : MonoBehaviour, IObject
{
    public Animator computerUI; //reference to the Computer UI
    public KeyboardController keyboard;

    [SerializeField] private int levelRequired; //the level required to decode the pass code
    [SerializeField] private string whatComputerDoes;   //what appears as the protocol on the computer UI
    [SerializeField] private string passCode;   //the pass code for this computer
    [SerializeField] private TMP_InputField inputField; //the input field to watch
    [SerializeField] private TextMeshProUGUI passCodeLabel; //the label that displays the code for the computer
    [SerializeField] private TextMeshProUGUI levelRequiredText; //label to display the required level for this computer
    [SerializeField] private TextMeshProUGUI protocolText;  //label to give the player a clue as to what this computer does
    [SerializeField] private AudioSource computerSound; //background noise for the computer

    public string Instructions => "Press E to Use Computer";

    public bool ShowInstructions => !computerUI.GetBool("computing");   //show the instructions if the player is not in the computer UI (computing = false)

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public bool ShakeCameraOnInteract => !computerUI.GetBool("computing");

    private bool playerTyping = false;    //used to disable interactions when typing
    private string codedChars = "!@#*&10";  //the coded characters
    public bool decoded = false;   //has the computer been decoded

    //delay objects
    private static WaitForSeconds sevenTenthsOfSecond = new WaitForSeconds(0.7f);
    private static WaitForSeconds fiveHundredsOfSecond = new WaitForSeconds(0.05f);

    void Start()
    {
        StartCoroutine(RefreshCodedCharacters());
    }

    void Update()
    {
        inputField.enabled = keyboard.KeyboardAvailable;    //can only type if you have a keyboard

        //stop the coroutine if the player has met the required level and has not decoded this computer
        if (keyboard.Level >= levelRequired && Vector2.Distance(transform.position, keyboard.transform.position) < 4f)
        {
            decoded = true;
        }

        //if the interaction with the computer is ending
        if (computerUI.GetCurrentAnimatorStateInfo(0).IsName("hide"))
        {
            computerSound.Stop();

            //the player is not interactong with the computer
            Controller.isInteracting = false;
        }
    }

    //enable interactions with the computer
    public void EnableInteract()
    {
        playerTyping = false;
    }

    //disable interactions with the computer
    public void DisableInteract()
    {
        playerTyping = true;
    }

    //check the pass code
    public void CheckPassCode()
    {
        //if the passcode is correct, execute the function
        if (inputField.text == passCode)
        {
            PassCodeCorrect();

            //add to the player context
            PlayerContext.AddComputerToContext(gameObject);

            inputField.textComponent.color = Color.green;
        }
        else{
            inputField.textComponent.color = Color.red;
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
        if (playerTyping) return;   //do nothing while the player is typing
        
        //make the input field blank
        inputField.textComponent.color = Color.black;
        inputField.text = "";

        AddEventListeners();

        //display the required level
        levelRequiredText.text = keyboard.FormatLevel(levelRequired) + " Required";

        //display the protocol
        protocolText.text = "Protocol: " + whatComputerDoes;

        //if computing = true (in the computer UI), set it to false and vice versa
        computerUI.SetBool("computing", !computerUI.GetBool("computing"));

        computerSound.Play();

        //the player is interacting with the computer
        Controller.isInteracting = true;
    }

    //method to add the relevant event listeners of this computer and remove all others
    //necessary to stop other computer instances checking for correct passwords and the like
    private void AddEventListeners(){
        inputField.onSelect.RemoveAllListeners();
        inputField.onSelect.AddListener(delegate { DisableInteract(); });

        inputField.onDeselect.RemoveAllListeners();
        inputField.onDeselect.AddListener(delegate { EnableInteract(); });

        inputField.onValueChanged.RemoveAllListeners();
        inputField.onValueChanged.AddListener(delegate { CheckPassCode(); });
    }

    //coroutine to repeatedly create a code above the computer
    private IEnumerator RefreshCodedCharacters()
    {
        //while the passcode is not decoded
        while(!decoded){
            string codedPassCode = "";

            //create a random code
            for (int i = 0; i < Random.Range(4, 8); i++)
            {
                //add random characters
                codedPassCode += codedChars[Random.Range(0, codedChars.Length - 1)];
            }

            passCodeLabel.text = codedPassCode; //set the text

            yield return sevenTenthsOfSecond;  //wait then repeat
        }

        //decode the passcode
        StartCoroutine(DecodePassCodeText());
    }

    //coroutine for the decoding animation
    private IEnumerator DecodePassCodeText(){
        string decodedPassCode = "";

        //go through each character in the passcode
        foreach(char c in passCode){
            decodedPassCode += c;

            string codedPassCode = "";

            //decoding each character takes between 2 and 4 attempts for each character
            for(int i = 0; i < Random.Range(2, 5); i++){
                codedPassCode = ""; //reset the coded section

                //for each of the remaining coded characters
                for (int j = 0; j < passCode.Length - decodedPassCode.Length; j++)
                {
                    //pad out the code with some coded characters
                    codedPassCode += codedChars[Random.Range(0, codedChars.Length - 1)];
                }

                //set the text of the passcode
                passCodeLabel.text = decodedPassCode + codedPassCode;

                yield return fiveHundredsOfSecond;
            }
        }

        passCodeLabel.text = passCode;
    }
}
