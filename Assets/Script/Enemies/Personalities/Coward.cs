using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coward : Personality
{
    WanderState wanderState;
    HideState hideState;
    Retreat retreatState;
    RefifllAmmo refillAmmoState;
    StunnedState stunnedState;


    public Coward(Enemy e) : base(e)
    {
        Init();
    }


    public override void Init()
    {
        base.Init();
        wanderState = new WanderState(this);
        hideState = new HideState(this);
        retreatState = new Retreat(this, enemyObj.health.maxHealth);
        refillAmmoState = new RefifllAmmo(this);
        stunnedState = new StunnedState(this);

        hideState.AddTransition(() => 
        {
            if(hideState.doneHiding)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Ok, done hiding");
                return true;
            }
            return false;
        }, wanderState);  

        //Will look for healthpack if health is not at max
        wanderState.AddTransition(() =>
        {
            if (enemyObj.health.GetHealth() < enemyObj.health.maxHealth)
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

        //Will look for healthpack if health is not at max
        wanderState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo < enemyObj.rifle.MaxAmmo)
            {
                var packs = GameManager.Instance.GetAvailableAmmoPacks();
                if (packs.Count > 0)
                {
                    if (stateMachine.GetCurrentState() != refillAmmoState)
                    {
                        refillAmmoState.allPacks = packs.ToArray();

                        Diagnostic.Instance.AddLog(enemyObj.gameObject, "Low ammo, got nothing to do, going to get some");
                        return true;
                    }
                }
            }
            return false;

        }, refillAmmoState);

        retreatState.AddTransition(() =>
        {
            var packs = GameManager.Instance.GetAvailableHealthPacks();
            //If the health was restored, or no packs available  go back wandering
            if (enemyObj.health.GetHealth() >= enemyObj.health.maxHealth ||
                enemyObj.health.GetHealth() < enemyObj.health.maxHealth && packs.Count == 0)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Feeling better, going to stroll around...");
                return true;
            }

            return false;
        }, wanderState);

        refillAmmoState.AddTransition(() =>
        {
            var packs = GameManager.Instance.GetAvailableAmmoPacks();
            //If the health was restored, or no packs available  go back wandering
            if (enemyObj.rifle.Ammo >= enemyObj.rifle.MaxAmmo ||
                enemyObj.rifle.Ammo < enemyObj.rifle.MaxAmmo && packs.Count == 0)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Feeling better, going to stroll around...");
                return true;
            }

            return false;
        }, wanderState);

        stunnedState.AddTransition(() =>
        {
            if (!stunnedState.isStunned)
            {
                return true;
            }

            return false;
        },
        hideState);

        stateMachine.SetState(wanderState);
    }

    public override void Update()
    {
        base.Update();
    }

  
    public override void OnPlayerSeen(Vector3 playerPos)
    {
        enemyObj.TriggerAlarm(playerPos);
        if(!hideState.isHiding)
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw the player, going to hide!");
            stateMachine.SetState(hideState);
        }

    }

    public override void OnGetBombed()
    {
        if (stateMachine.GetCurrentState() != stunnedState && stateMachine.GetCurrentState() != stunnedState)
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Got bombed!");
            stateMachine.SetState(stunnedState);
        }
    }

    public override void OnGetShot(GameObject from)
    {

        if (from.gameObject.tag.Equals("Player"))
        {
            if (!hideState.isHiding && stateMachine.GetCurrentState() != stunnedState)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "I got shot! Sending SOS and going to hide!");

                enemyObj.TriggerAlarm(enemyObj.transform.position);
                stateMachine.SetState(hideState);

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
        float toPlayer = (shotPosition - enemyObj.transform.position).magnitude;

        if (toPlayer < enemyObj.enemySight.hearRange)
        {

            if (!hideState.isHiding)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Heard the player, going to hide!");
                stateMachine.SetState(hideState);
            }
        }
    }

}
