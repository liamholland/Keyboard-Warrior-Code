using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//conversation that levels up the keyboard afterwards
[CreateAssetMenu(fileName = "newConversationLevelUp", menuName = "Conversation With Action/Level Up")]
public class ConversationLevelUp : Conversation
{
    public override bool HasAction => true;

    public override void DoAction()
    {
        //level up the keyboard
        KeyboardController keyboard = GameObject.Find("Keyboard").GetComponent<KeyboardController>();
        keyboard.Level++;
    }
}
