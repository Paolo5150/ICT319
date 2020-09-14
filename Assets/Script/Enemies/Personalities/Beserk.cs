using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beserk : Personality
{
    WanderState wanderState;
    ShootState shootState;
    InvestigateState investigationState;
    StunnedState stunnedState;
    MeleeAttackState meleeAttackState;
    RefifllAmmo refillAmmoState;
    Retreat retreatState;
    Sprite qMark;

    float shootTimer = 0;
    float minHealthForRetreat = 65.0f;

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
        meleeAttackState = new MeleeAttackState(this);
        refillAmmoState = new RefifllAmmo(this);
        retreatState = new Retreat(this, minHealthForRetreat);

        qMark = Resources.Load<Sprite>("StateIcons\\what");

        wanderState.AddTransition(() =>
        {
            if(enemyObj.enemySight.IsPlayerInSight() && enemyObj.rifle.Ammo > 0)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "See the player, shooting!");

                return true;
            }
            return false;

        }, shootState);

        wanderState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo <= 0)
            {
                var packs = GameManager.Instance.GetAvailableAmmoPacks();
                if (packs.Count > 0)
                {
                    if (stateMachine.GetCurrentState() != refillAmmoState)
                    {
                        refillAmmoState.allPacks = packs.ToArray();
                        Diagnostic.Instance.AddLog(enemyObj.gameObject, "No ammo, got nothing to do, going to get some");
                        return true;
                    }
                }
            }
            return false;

        }, refillAmmoState);

        wanderState.AddTransition(() =>
        {
            if (enemyObj.health.GetHealth() < minHealthForRetreat)
            {
                var packs = GameManager.Instance.GetAvailableHealthPacks();
                if (packs.Count > 0)
                {
                    if (stateMachine.GetCurrentState() != retreatState)
                    {
                        retreatState.allPacks = packs.ToArray();

                        Diagnostic.Instance.AddLog(enemyObj.gameObject, "Low health, got nothing to do, going to get some");
                        return true;
                    }
                }
            }
            return false;

        }, retreatState);

        wanderState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo <= 0)
            {
                var packs = GameManager.Instance.GetAvailableAmmoPacks();
                if (packs.Count > 0)
                {
                    if (stateMachine.GetCurrentState() != refillAmmoState)
                    {
                        Diagnostic.Instance.AddLog(enemyObj.gameObject, "No ammo, got nothing to do, going to get some");
                        return true;
                    }
                }
            }
            return false;

        }, refillAmmoState);

        wanderState.AddTransition(() =>
        {
            if (enemyObj.enemySight.IsPlayerInSight() && enemyObj.rifle.Ammo <= 0)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "See the player, no ammo, will attack with hands!");

                return true;
            }
            return false;

        }, meleeAttackState);

        refillAmmoState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo > 0)
            {
                if (refillAmmoState.playerLastKnowsPosition == null)
                {
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Go ammo, don't know where player is, will wander around");
                    return true;
                }
            }

            return false;
        }, wanderState);

        refillAmmoState.AddTransition(() =>
        {
            if (enemyObj.enemySight.IsPlayerInSight())
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw player, no time for refill ammo, attack!");
                return true;
            }

            return false;
        }, meleeAttackState);

        retreatState.AddTransition(() =>
        {
            if (enemyObj.enemySight.IsPlayerInSight())
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw player, no time for get healthpack, attack!");
                return true;
            }

            return false;
        }, shootState);

        retreatState.AddTransition(() =>
        {
            //If the health was restored OR health is low but no packs available, go back wandering
            if (enemyObj.health.GetHealth() > minHealthForRetreat ||
                   (enemyObj.health.GetHealth() <= minHealthForRetreat && GameManager.Instance.GetAvailableHealthPacks().Count == 0) )
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Feeling better, going to stroll around...");
                return true;
            }

            return false;
        }, wanderState);

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

        shootState.AddTransition(() => {

            return enemyObj.rifle.Ammo <= 0;
        }, meleeAttackState);

        meleeAttackState.AddTransition(() => {

            if (meleeAttackState.playerLastKnownPosition != null)
            {
                investigationState.investigationPoint = meleeAttackState.playerLastKnownPosition.Value;
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

        stunnedState.AddTransition(() =>
        {
            if (!stunnedState.isStunned)
            {
                investigationState.investigationPoint = enemyObj.transform.position;

                return true;
            }

            return false;
        },
        investigationState);


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
        if (stateMachine.GetCurrentState() == stunnedState)
            return;
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
      /*  if(shootTimer > 0 && stateMachine.GetCurrentState() != stunnedState)
        {
            enemyObj.rifle.Shoot(enemyObj.shootRate, enemyObj.gameObject, enemyObj.damageGiven);
            shootTimer -= Time.deltaTime;
        }*/
    }

    public override void OnGetBombed()
    {
        if(stateMachine.GetCurrentState() != stunnedState)
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Got bombed!");

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

    public override void OnPlayerSeen(Vector3 pPosition)
    {

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
