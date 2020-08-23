using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beserk : Personality
{
    WanderState wanderState;
    ShootState shootState;
    InvestigateState investigationState;
    StunnedState stunnedState;
    Sprite qMark;

    float shootTimer = 0;

    public Beserk(Enemy e) : base(e)
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        wanderState = new WanderState(this);
        shootState = new ShootState(this, enemyObj.shootRate);
        investigationState = new InvestigateState(this);
        stunnedState = new StunnedState(this);

        qMark = Resources.Load<Sprite>("StateIcons\\what");

        wanderState.AddTransition(() =>
        {
            if(enemyObj.enemySight.IsPlayerInSight())
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "See the player, shooting!");

                return true;
            }
            return false;

        }, shootState);

        shootState.AddTransition(()=> 
        {
            if(!enemyObj.enemySight.IsPlayerInSight())
            {
                investigationState.waitBeforeGoingToPoint = 0;
                investigationState.investigationPoint = Player.Instance.transform.position;
                shootTimer = 15.0f;
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Lost sight of player, going to check last known psition (and keep shooting, why not)");

                return true;
            }
            return false;
        }, investigationState);

        investigationState.AddTransition(() => {
            if (enemyObj.enemySight.IsPlayerInSight())
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "See the player, shooting!");

                return true;
            }
            return false;
        },shootState);

        investigationState.AddTransition(() => {
            return investigationState.done;
        }, wanderState);

        stunnedState.AddTransitionDynamicState(() =>
        {
            return !stunnedState.isStunned;
        },
        () =>
        {
            return stateMachine.previousState;
        });


        //Beserk will respond to alarm
        Enemy.OnAlarmSent += GoInsestigateSOS;
        stateMachine.SetState(wanderState);
    }

    public override void OnObjDisable()
    {
        Enemy.OnAlarmSent -= GoInsestigateSOS;

    }

    void GoInsestigateSOS(Vector3 pos, GameObject triggeredBy)
    {
        investigationState.waitBeforeGoingToPoint = 0.0f;
        investigationState.investigationPoint = pos;

        if (stateMachine.GetCurrentState() == investigationState)
        {
            if (!investigationState.isInvestigating)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Received an SOS, going to check");
                stateMachine.SetState(investigationState);
            }
        }
        else
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Received an SOS, going to check");
            stateMachine.SetState(investigationState);
        }
    }

    public override void Update()
    {
        base.Update();
        //Will shot for a certain amount of time after losing sight of the player
        if(shootTimer > 0 && stateMachine.GetCurrentState() != stunnedState)
        {
            enemyObj.rifle.Shoot(enemyObj.shootRate, enemyObj.gameObject, enemyObj.damageGiven);
            shootTimer -= Time.deltaTime;
        }
    }

    public override void OnGetBombed()
    {
        if(stateMachine.GetCurrentState() != stunnedState)
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Great, stepped on a bomb...");

            stateMachine.SetState(stunnedState);
        }
    }


    public override void OnGetShot(GameObject from)
    {
        if(from.tag.Equals("Player"))
        {
            if (stateMachine.GetCurrentState() != shootState && !investigationState.isInvestigating)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Got shot by player, going to check");

                investigationState.investigationPoint = from.transform.position;
                stateMachine.SetState(investigationState);
            }
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
        if(stateMachine.GetCurrentState() == investigationState)
            investigationState.waitBeforeGoingToPoint = 0.0f;
            else
            investigationState.waitBeforeGoingToPoint = 1.0f;

        if (stateMachine.GetCurrentState() != shootState && !investigationState.isInvestigating)
        {
            float toPlayer = (shotPosition - enemyObj.transform.position).magnitude;

            if (toPlayer < enemyObj.enemySight.hearRange)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Heard the player shooting nearby, going to check");

                enemyObj.stateIcon.EnableTemporarily(qMark);
                enemyObj.navigator.Stop();
                investigationState.investigationPoint = shotPosition;
                stateMachine.SetState(investigationState);

            }
        }
       
    }
}
