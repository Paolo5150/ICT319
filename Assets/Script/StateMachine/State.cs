using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void OnEnter();
    public abstract void Update();
    public abstract void OnExit();

    public void AddTransition(Func<bool> condition, State nextState)
    {
        if (transitions == null)
            transitions = new Dictionary<Func<bool>, State>();

        transitions.Add(condition, nextState);

    }

    protected Dictionary<Func<bool>, State> transitions;
     

}
