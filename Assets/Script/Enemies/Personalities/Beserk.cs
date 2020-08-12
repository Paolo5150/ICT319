using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beserk : Personality
{
    WanderState wanderState;
    ShootState shootState;
    InvestigateState investigationState;

    //Test


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

    IEnumerator GoInvestigateNoise(Vector3 shotPosition)
    {
        enemyObj.transform.LookAt(shotPosition);
        yield return new WaitForSeconds(3);
        investigationState.investigationPoint = shotPosition;
        stateMachine.SetState(investigationState);
    }

    public override void OnPlayeShotFired(Vector3 shotPosition)
    {
        float toPlayer = (shotPosition - enemyObj.transform.position).magnitude;

        if(toPlayer < enemyObj.enemySight.hearRange)
        {
            enemyObj.navigator.Stop();
            enemyObj.StopAllCoroutines();
            enemyObj.StartCoroutine(GoInvestigateNoise(shotPosition));

        }
    }
}
