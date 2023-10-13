using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour, IObject
{
    public Conversation defaultConversation;
    public Conversation[] conversations;
    public TextMeshProUGUI dialougeBox;
    public TextMeshProUGUI speakerName;
    public Animator dialogueBoxAnimator;

    private int currentLineIndex;
    private Conversation currentConvo;
    private bool isTalking = false;

    public bool IsTalking {
        get => isTalking;
        set {
            isTalking = value;

            //set the value of the dialogue box
            dialogueBoxAnimator.SetBool("inConversation", value);
        }
    }

    private void Start()
    {
        //make each conversation available at the start if it needs to be
        foreach (Conversation c in conversations)
            c.isAvailable = c.availableAtStart;
    }

    public string Instructions
    {
        get
        {
            if (isTalking)
            {
                return "Press E to Continue";
            }
            else
            {
                return "Press E to Talk";
            }
        }
    }

    //called once for each interaction
    public void Do()
    {
        if (isTalking)
        {
            currentLineIndex++;
            RunConversation(currentConvo);
        }
        else
        {
            foreach (Conversation c in conversations)
            {
                if (c.isAvailable)
                {
                    currentLineIndex = 0;
                    IsTalking = true;
                    currentConvo = c;
                    RunConversation(c);
                    return;
                }
            }
            currentLineIndex = 0;
            IsTalking = true;   //animates the dialogue box as well
            currentConvo = defaultConversation;
            RunConversation(defaultConversation);
        }
    }

    private void RunConversation(Conversation c)
    {
        if(currentLineIndex >= c.conversationLines.Length)
        {
            c.isAvailable = false;
            IsTalking = false;
            dialogueBoxAnimator.SetBool("animTalking", false);
            dialougeBox.text = "";
            speakerName.text = "";
            return;
        }
        else
        {
            dialougeBox.text = c.conversationLines[currentLineIndex].line;
            speakerName.text = c.conversationLines[currentLineIndex].speakerName;
        }
    }
}
