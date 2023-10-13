using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newConversationAnimatorAction", menuName = "Conversation With Action/Animator Action")]
public class ConversationWithAnimatorAction : Conversation
{

    [SerializeField] private AnimatorSettings[] animators;  //list of animators and the values to set

    public override bool HasAction => true; //this class has an action to perform

    private Animator animator;   //animator to perform action on

    public override void DoAction()
    {
        //set the parameters on the various animators
        foreach(AnimatorSettings animator in animators){
            switch(animator.type){
                case AnimatorSettings.ParameterType.BOOL:
                    SetAnimatorBoolParameter(animator.animatorGameObjectName, animator.parameterName, bool.Parse(animator.value));
                    break;
                default:
                    break;
            }
        }
    }

    private void SetAnimatorBoolParameter(string gameObjectName, string paramName, bool value){
        //find the animator
        animator = GameObject.Find(gameObjectName).GetComponent<Animator>();

        if(animator != null){
            animator.SetBool(paramName, value);
        }
    }

    //class to define animators
    [Serializable]
    private class AnimatorSettings{
        public string animatorGameObjectName;   //the name of the game object with the animator on it
        public string parameterName;    //the name of a parameter to change
        public ParameterType type;
        public string value;  //value to set boolean parameters to
        
        //list of primitive types used in animators
        public enum ParameterType{
            BOOL,
        }
    }
}
