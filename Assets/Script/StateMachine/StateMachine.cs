using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : State
{
    // Update is called once per frame

    T currentState;

    public T previousState;
    public void SetState(T newState)
    {

        if (currentState != null)
            currentState.OnExit();

        previousState = currentState;
        currentState = newState;

        currentState.OnEnter();

    }

    public void Update()
    {
        if (currentState != null)
            currentState.Update();
    }

    public T GetCurrentState()
    {
        return currentState;
    }
}
