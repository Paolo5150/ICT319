using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beserk : Personality
{
    WanderState wanderState;
    ShootState shootState;
    InvestigateState investigationState;

    float shootRate = 0.1f;

    public Beserk(Enemy e) : base(e)
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        wanderState = new WanderState(this);
        shootState = new ShootState(this, shootRate);
        investigationState = new InvestigateState(this);



        wanderState.AddTransition(() =>
        {
            return enemyObj.enemySight.IsPlayerInSight();

        }, shootState);

        shootState.AddTransition(()=> 
        {
            if(!enemyObj.enemySight.IsPlayerInSight())
            {
                investigationState.investigationPoint = Player.Instance.transform.position;
                return true;
            }
            return false;
        }, investigationState);

        investigationState.AddTransition(() => {
            return enemyObj.enemySight.IsPlayerInSight();
        },shootState);

        investigationState.AddTransition(() => {
            return investigationState.done;
        }, wanderState);

        stateMachine.SetState(wanderState);
    }

    public override void Update()
    {
        base.Update();
    }
}
