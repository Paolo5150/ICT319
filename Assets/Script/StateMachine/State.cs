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

    public void AddTransitionDynamicState(Func<bool> condition, Func<State> funcToState)
    {
        if (transitionsDynamicState == null)
            transitionsDynamicState = new Dictionary<Func<bool>, Func<State>>();

        transitionsDynamicState.Add(condition, funcToState);

    }
    protected Dictionary<Func<bool>, State> transitions;
    protected Dictionary<Func<bool>, Func<State>> transitionsDynamicState;
     

}
