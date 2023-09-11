using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newConversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public ConversationLine[] conversationLines;
    public bool availableAtStart;
    [HideInInspector] public bool isAvailable;
}
