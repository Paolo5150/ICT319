using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beserk : Enemy
{
    StateMachine<EnemyState<Beserk>> stateMachine;

    public Idle idleState;
    public Wander wanderState;

    public override void Start()
    {
        base.Start();
        wanderState = new Wander(this);

        stateMachine = new StateMachine<EnemyState<Beserk>>();
        stateMachine.SetState(wanderState);

    }
    public void Update()
    {
        stateMachine.Update();
    }

}
