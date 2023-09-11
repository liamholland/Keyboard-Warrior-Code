using UnityEngine;
using UnityEngine.UI;

public class Npc : IObject
{
    public Conversation defaultConversation;
    public Conversation[] conversations;
    public Text dialougeBox;
    public Text speakerName;
    public Animator dBoxAnimator;

    private int currentLineIndex;
    private Conversation currentConvo;

    public static bool isTalking = false;

    private void Start()
    {
        foreach (Conversation c in conversations)
            c.isAvailable = c.availableAtStart;
    }

    public override string Instructions
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

    public override void Do()
    {
        if (isTalking)
        {
            currentLineIndex++;
            Convo(currentConvo);
        }
        else
        {
            foreach (Conversation c in conversations)
            {
                if (c.isAvailable)
                {
                    currentLineIndex = 0;
                    isTalking = true;
                    currentConvo = c;
                    dBoxAnimator.SetBool("animTalking", true);
                    Convo(c);
                    return;
                }
            }
            currentLineIndex = 0;
            isTalking = true;
            currentConvo = defaultConversation;
            dBoxAnimator.SetBool("animTalking", true);
            Convo(defaultConversation);
        }
    }

    private void Convo(Conversation c)
    {
        if(currentLineIndex >= c.conversationLines.Length)
        {
            c.isAvailable = false;
            isTalking = false;
            dBoxAnimator.SetBool("animTalking", false);
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
