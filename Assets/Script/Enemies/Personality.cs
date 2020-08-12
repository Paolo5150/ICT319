using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Personality
{
    public StateMachine<EnemyState> stateMachine;
    public Personality(Enemy e)
    {
        enemyObj = e;
    }
    public virtual void Init()
    {
        stateMachine = new StateMachine<EnemyState>();
    }
    public virtual void Update()
    {
        stateMachine.Update();
    }

    public virtual void OnPlayeShotFired(Vector3 shotPosition)
    { }


    public Enemy enemyObj;

}
