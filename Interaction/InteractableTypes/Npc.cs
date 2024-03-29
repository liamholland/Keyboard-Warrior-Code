﻿using TMPro;
using UnityEngine;

public class Npc : MonoBehaviour, IObject
{
    public Conversation defaultConversation;    //npcs can have a conversation that will always run
    public Conversation[] conversations;    //a list of conversation the npc can have
    public TextMeshProUGUI dialougeBox;
    public TextMeshProUGUI speakerName;
    [SerializeField] private Animator dialogueBoxAnimator;
    [SerializeField] private AudioSource talkingSound;  //the sound played when talking

    private int currentLineIndex;
    private Conversation currentConvo;
    private bool isTalking = false;

    //public accessor for isTalking
    public bool IsTalking {
        get => isTalking;
        set {
            isTalking = value;

            Controller.isInteracting = value;

            //set the value of the dialogue box
            dialogueBoxAnimator.SetBool("inConversation", value);
        }
    }

    private void Start()
    {
        //make each conversation available at the start if it needs to be
        foreach (Conversation c in conversations){
            if(!(PlayerContext.conversationsToMakeAvailable.Contains(c.name) ||
                PlayerContext.conversationsToMakeUnavailable.Contains(c.name))){
                    c.IsAvailable = c.availableAtStart;
                }
        }
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
                return "Press E to Listen";
            }
        }
    }

    public bool ShowInstructions => true;

    public bool UseDefaultInteractButton => true;

    public string CustomKeyCode => "";

    public bool ShakeCameraOnInteract => !isTalking;

    //called once for each interaction by the player
    public void Do()
    {   
        //if the npc is talking
        if (IsTalking)
        {
            currentLineIndex++; //increment the line index
            DisplayConversationLine(currentConvo);  //display the line
        }
        else    //start a conversation
        {
            talkingSound.Play();

            currentLineIndex = 0;   //set index to first line

            //for each conversation
            foreach (Conversation c in conversations)
            {
                if (c.IsAvailable)
                {
                    IsTalking = true;
                    currentConvo = c;
                    DisplayConversationLine(c);
                    return;
                }
            }

            //if there are no conversations available, use the default one
            IsTalking = true;   //animates the dialogue box as well
            currentConvo = defaultConversation;
            DisplayConversationLine(defaultConversation);
        }

        Controller.isInteracting = IsTalking;
    }

    //function to display lines from conversations
    private void DisplayConversationLine(Conversation c)
    {
        //if the conversation is finished
        if(currentLineIndex >= c.conversationLines.Length)
        {
            //set the default conversation to this conversation if it is meant to do that
            if(c.makeDefaultAfterFinished){
                defaultConversation = c;
            }

            //if the conversation has an action, execute it when the conversation is finished
            if(c.HasAction){
                c.DoAction();
            }

            //if the npc is done in the scene after this conversation, record that in the context
            if(c.npcDoneInScene){
                PlayerContext.AddNpcDoneInSceneToContext(gameObject);
            }

            c.IsAvailable = false;  //mark the conversation as unavailable

            IsTalking = false;  //the npc is no longer talking

            //make the dialogue box blank
            dialougeBox.text = "";
            speakerName.text = "";
        }
        else
        {
            dialougeBox.text = c.conversationLines[currentLineIndex].line;  //set the line
            speakerName.text = c.conversationLines[currentLineIndex].speakerName;   //set the name in case its the first line
        }
    }
}
