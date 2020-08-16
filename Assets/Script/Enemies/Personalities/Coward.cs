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

  
    public override void OnPlayerSeen(Vector3 playerPos)
    {

        Enemy.TriggerAlarm(playerPos);
        hideState.useSosIcon = true;
        stateMachine.SetState(hideState);
    }

    public override void OnGetShot(GameObject from)
    {
        if(from.gameObject.tag.Equals("Player"))
        {
            Enemy.TriggerAlarm(enemyObj.transform.position);
            hideState.useSosIcon = true;
            stateMachine.SetState(hideState);
        }

    }
    public override void OnPlayerDeath()
    {
        base.OnPlayerDeath();
        enemyObj.enemySight.enabled = false;
        stateMachine.SetState(wanderState);
    }
    public override void OnPlayeShotFired(Vector3 shotPosition)
    {
        float toPlayer = (shotPosition - enemyObj.transform.position).magnitude;

        if (toPlayer < enemyObj.enemySight.hearRange)
        {
            hideState.useSosIcon = false;

            if (!hideState.isHiding)
            stateMachine.SetState(hideState);
        }
    }
}
