using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newConversationAnimatorAction", menuName = "Conversation With Action/Animator Action")]
public class ConversationWithAnimatorAction : Conversation
{

    [SerializeField] private AnimatorParameterSettings[] animators;  //list of animators and the values to set

    public override bool HasAction => true; //this class has an action to perform

    private Animator animator;   //animator to perform action on

    public override void DoAction()
    {
        //set the parameters on the various animators
        foreach(AnimatorParameterSettings animator in animators){
            switch(animator.type){
                case AnimatorParameterSettings.ParameterType.BOOL:
                    SetAnimatorBoolParameter(animator.animatorGameObjectName, animator.parameterName, bool.Parse(animator.value));
                    break;
                default:
                    break;
            }
        }
    }

    //function for setting a boolean parameter
    private void SetAnimatorBoolParameter(string gameObjectName, string paramName, bool value){
        //get the game object
        GameObject animatorObject = GameObject.Find(gameObjectName);
        
        if(animatorObject != null){
            //find the animator
            animator = animatorObject.GetComponent<Animator>();

            if(animator != null){
                animator.SetBool(paramName, value);
            }

            //check if the object is a door
            IDoor door = animatorObject.GetComponent<IDoor>();
            if(door != null && value == true){
                //add the door to the context if it is and the door is open
                PlayerContext.AddDoorToContext(animatorObject);
            }
        }
    }

    //class to define animators
    [Serializable]
    private class AnimatorParameterSettings{
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
