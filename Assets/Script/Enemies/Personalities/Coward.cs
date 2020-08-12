using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coward : Personality
{
    WanderState wanderState;
    HideState hideState;


    public Coward(Enemy e) : base(e)
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        wanderState = new WanderState(this);
        hideState = new HideState(this);

        hideState.AddTransition(() => 
        {
            return hideState.doneHiding;
        }, wanderState);

        stateMachine.SetState(wanderState);
    }

    public override void Update()
    {
        base.Update();
    }

  

    public override void OnPlayeShotFired(Vector3 shotPosition)
    {
        stateMachine.SetState(hideState);
    }
}
