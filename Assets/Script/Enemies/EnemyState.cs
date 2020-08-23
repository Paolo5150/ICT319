using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : State
{
    public EnemyState(Personality p)
    {
        personalityObj = p;
    }

    public Personality personalityObj;
    public Sprite stateImageSprite;
    public override void OnEnter()
    {
        Diagnostic.Instance.AddLog(personalityObj.enemyObj.gameObject,"Entering state " + stateName);
        if(stateImageSprite != null)
            personalityObj.enemyObj.stateIcon.EnableTemporarily(stateImageSprite);

    }
    public override void Update()
    {
        //Check conditions
        if(transitions != null)
        {
            foreach (KeyValuePair<Func<bool>, State> condition in transitions)
            {
                if (condition.Key())
                {
                    personalityObj.stateMachine.SetState((EnemyState)condition.Value);
                }
            }    
        }

        if(transitionsDynamicState != null)
        {
            foreach (KeyValuePair<Func<bool>, Func<State>> condition in transitionsDynamicState)
            {
                if (condition.Key())
                {
                    personalityObj.stateMachine.SetState((EnemyState)condition.Value());
                }
            }
        }

    }
    public override void OnExit()
    {
        Diagnostic.Instance.AddLog(personalityObj.enemyObj.gameObject, "Exiting state " + stateName);
    }
    public string stateName;


}
