using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newConversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public ConversationLine[] conversationLines;
    public bool availableAtStart;
    public bool makeDefaultAfterFinished;   //should the npc repeat this conversation after they have finished saying it
    [HideInInspector] public bool isAvailable;
    public virtual bool HasAction => false;  //base conversations do not have an action

    //can execute if it is set to have an action
    public virtual void DoAction(){ return; }
}
