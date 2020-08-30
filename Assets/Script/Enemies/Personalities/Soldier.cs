using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : Personality
{
    WanderState wanderState;
    ShootState shootState;
    InvestigateState investigationState;
    Retreat retreatState;
    HideState hideState;
    RefifllAmmo refillAmmoState;
    BombAvoid bombAvoidState;

    float minHealthForRetreat = 35.0f;
    float alarmTimer = 0;


    public Soldier(Enemy e) : base(e)
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        wanderState = new WanderState(this);
        shootState = new ShootState(this, enemyObj.shootRate);
        investigationState = new InvestigateState(this);
        retreatState = new Retreat(this, minHealthForRetreat);
        hideState = new HideState(this);
        bombAvoidState = new BombAvoid(this);
        refillAmmoState = new RefifllAmmo(this);

        shootState.AddTransition(() =>
        {
            if (!enemyObj.enemySight.IsPlayerInSight())
            {
                investigationState.waitBeforeGoingToPoint = 0;
                investigationState.investigationPoint = Player.Instance.transform.position;
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Lost sight of player, going to check last known position");

                return true;
            }
            return false;
        }, investigationState);

        shootState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo <= 0)
            {
                var allPacks = GameManager.Instance.GetAvailableAmmoPacks();
                if(allPacks.Count == 0)
                {
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Out of ammo, no packs around, going to hide!");
                    return true;

                }

            }
            return false;
        }, hideState);

        shootState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo <= 0)
            {
                var allPacks = GameManager.Instance.GetAvailableAmmoPacks();
                if (allPacks.Count > 0)
                {
                    refillAmmoState.allPacks = allPacks.ToArray();
                    refillAmmoState.playerLastKnowsPosition = Player.Instance.transform.position;
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Out of ammo, going to refill!");
                    return true;
                }

            }
            return false;
        }, refillAmmoState);

        investigationState.AddTransition(() => {
            return investigationState.done;
        }, wanderState);

        refillAmmoState.AddTransition(() =>
        {
            if(enemyObj.rifle.Ammo <=0 )
            {
                var allPacks = GameManager.Instance.GetAvailableAmmoPacks();
                if (allPacks.Count == 0)
                {
                    return true;
                }
            }
   
            return false;
        }  , hideState);

        refillAmmoState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo > 0)
            {
                if(refillAmmoState.playerLastKnowsPosition != null)
                {
                    investigationState.investigationPoint = refillAmmoState.playerLastKnowsPosition.Value;
                    return true;
                }
            }

            return false;
        }, investigationState);

        refillAmmoState.AddTransition(() =>
        {
            if (enemyObj.rifle.Ammo > 0)
            {
                if (refillAmmoState.playerLastKnowsPosition == null)
                {
                    return true;
                }
            }

            return false;
        }, wanderState);

        //If health is ok and retreat has a player known pos, go investigate
        retreatState.AddTransition(() => {

            if(enemyObj.health.GetHealth() >= minHealthForRetreat)
            {
                if (retreatState.playerLastKnowsPosition != null)
                    investigationState.investigationPoint = retreatState.playerLastKnowsPosition.Value;

                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Feeling better, go to check last investigation point");

                return true;
            }

            return false;
        }, investigationState);

        //If health is ok and retreat has no player known pos, go patrol

        retreatState.AddTransition(() => {

            if (enemyObj.health.GetHealth() >= minHealthForRetreat)
            {
                if (retreatState.playerLastKnowsPosition == null)
                {
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Feeling better, go to back to wander");
                    return true;
                }
            }

            return false;
        }, wanderState);
               
        retreatState.AddTransition(() =>
        {

            if (enemyObj.health.GetHealth() < minHealthForRetreat)
            {
                var packs = GameManager.Instance.GetAvailableHealthPacks();

                if (packs.Count == 0)
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Go a healthpack, but still feel crap, will go hide");

                return packs.Count == 0 && !hideState.isHiding;
            }
            return false;
        }, hideState);


        //Will come out of hiding only when a health pack is available again
        hideState.AddTransition(() => {

            var packs = GameManager.Instance.GetAvailableHealthPacks();

            if (packs.Count > 0 && enemyObj.health.GetHealth() < retreatState.minHealthForRetreat)
            {
                retreatState.allPacks = packs.ToArray();
                return true;
            }

            return false;
        }, retreatState);

        //Will come out of hiding only when a ammo pack is available again
        hideState.AddTransition(() => {

            var packs = GameManager.Instance.GetAvailableAmmoPacks();
            if (packs.Count > 0 && enemyObj.health.GetHealth() >= retreatState.minHealthForRetreat)
            {
                refillAmmoState.allPacks = packs.ToArray();
                return true;
            }

            return false;
        }, refillAmmoState);

        bombAvoidState.AddTransitionDynamicState(() =>
        {
            return bombAvoidState.readyToGo;
        }, 
        ()=> { return stateMachine.previousState; });

        //Soldier will respond to alarm
        Enemy.OnAlarmSent += GoInvestigateSOS;
        stateMachine.SetState(wanderState);
    }

    public override void OnObjDisable()
    {
        Enemy.OnAlarmSent -= GoInvestigateSOS;

    }

    void GoInvestigateSOS(Vector3 pos, GameObject triggeredBy)
    {
        if(triggeredBy == enemyObj.gameObject ||
            stateMachine.GetCurrentState() == retreatState ||
               stateMachine.GetCurrentState() == hideState)
        {
            //Debug.Log("Received alarm, but will ignore!");
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Received an SOS, but will ignore");
            return;
        }
        investigationState.waitBeforeGoingToPoint = 0.0f;
        investigationState.investigationPoint = pos;

        if (stateMachine.GetCurrentState() == investigationState)
        {
            if (!investigationState.isInvestigating)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Received an SOS, will go check");
                stateMachine.SetState(investigationState);
            }
        }
        else
        {
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Received an SOS, will go check");
            stateMachine.SetState(investigationState);
        }
    }



    public override void Update()
    {
        base.Update();
        if (alarmTimer > 0)
        {
            alarmTimer -= Time.deltaTime;
        }

        float distToBomb = enemyObj.enemySight.IsObjectInSight(Player.Instance.bomb.gameObject);
        if (distToBomb != -1 && distToBomb <= 2)
        {
            if (stateMachine.GetCurrentState() != bombAvoidState && Player.Instance.bomb.gameObject.transform.position != bombAvoidState.lastBombPosition)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "I see the bomb, will find alternative path");

                bombAvoidState.lastBombPosition = Player.Instance.bomb.gameObject.transform.position;
                stateMachine.SetState(bombAvoidState);
            }

        }

    }


    public override void OnGetShot(GameObject from)
    {
        if(enemyObj.health.GetHealth() < minHealthForRetreat)
        {
            //If shot by player, remember last known position
            if (from.gameObject.tag.Equals("Player"))
                retreatState.playerLastKnowsPosition = from.gameObject.transform.position;
            else
                retreatState.playerLastKnowsPosition = null;
            EvaluateRetreat();
            return;
        }
        else
        {
            if (from.tag.Equals("Player"))
            {
                if(enemyObj.rifle.Ammo > 0)
                {
                    if (stateMachine.GetCurrentState() != shootState && !investigationState.isInvestigating)
                    {
                        Diagnostic.Instance.AddLog(enemyObj.gameObject, "Got shot by player! I'll go check where the shot came from");

                        investigationState.investigationPoint = from.transform.position;
                        stateMachine.SetState(investigationState);
                    }
                }
                else
                {
                    EvaluateAmmoRefill();

                }

            }
        }


    }

    public override void OnPlayerSeen(Vector3 pPosition)
    {
        base.OnPlayerSeen(pPosition);
        if(alarmTimer <= 0.0f)
        {
           // Debug.Log("Alarm triggered");
            Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw the player! Sending SOS");

            enemyObj.TriggerAlarm(pPosition);
            alarmTimer = 5.0f;
        }

        if (enemyObj.health.GetHealth() >= minHealthForRetreat)
        {
            if(enemyObj.rifle.Ammo > 0)
            {
                if (stateMachine.GetCurrentState() != shootState )
                {
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw the player, shooting!");

                    stateMachine.SetState(shootState);
                }
            }
            else
            {
                EvaluateAmmoRefill();
            }

        }
        else
        {
            retreatState.playerLastKnowsPosition = pPosition; //Remember player host position, so if he gets a health pack, will go check

            EvaluateRetreat();

            if(stateMachine.previousState != hideState && stateMachine.GetCurrentState() == hideState)
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw the player, but will go hide!");

            if (stateMachine.previousState != retreatState && stateMachine.GetCurrentState() == retreatState)
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "Saw the player, but will go get healthpack!");

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

        if (stateMachine.GetCurrentState() == investigationState)
            investigationState.waitBeforeGoingToPoint = 0.0f;
        else
            investigationState.waitBeforeGoingToPoint = 1.0f;

        float toPlayer = (shotPosition - enemyObj.transform.position).magnitude;
        if (toPlayer > enemyObj.enemySight.hearRange)
            return;


        if (enemyObj.health.GetHealth() >= minHealthForRetreat)
        {
            if(enemyObj.rifle.Ammo > 0)
            {
                if (stateMachine.GetCurrentState() != shootState && !investigationState.isInvestigating)
                {
                    Diagnostic.Instance.AddLog(enemyObj.gameObject, "I heard the player shooting nearby, I'll go check out");

                    enemyObj.navigator.Stop();
                    investigationState.investigationPoint = shotPosition;
                    stateMachine.SetState(investigationState);
                }
            }
            else
            {
                EvaluateAmmoRefill();
            }

        }
        else
        {

            retreatState.playerLastKnowsPosition = shotPosition; //Remember player host position, so if he gets a health pack, will go check
            EvaluateRetreat();
        }
       

    }

    public override void OnTriggerEnter(Collider c)
    {
        if(stateMachine.GetCurrentState() == retreatState)
        {
            if(c.gameObject.tag.Equals("Healthpack"))
            {
                enemyObj.health.Add(Healthpack.HEALTH_GIVEN);
                c.gameObject.GetComponent<Healthpack>().Reset();
                Diagnostic.Instance.UpdateHealth(enemyObj.gameObject, enemyObj.health.GetHealth());
            }
        }
    }
       
    private void EvaluateAmmoRefill()
    {
        var packs = GameManager.Instance.GetAvailableAmmoPacks();
        if (packs.Count > 0)
        {
            if (stateMachine.GetCurrentState() != refillAmmoState)
                stateMachine.SetState(refillAmmoState);

        }
        else
        {
            if (!hideState.isHiding)
                stateMachine.SetState(hideState);
        }
    }
    private void EvaluateRetreat()
    {

        var packs = GameManager.Instance.GetAvailableHealthPacks();

        //If there's an active healthpack, go get it
        if (packs.Count > 0)
        {
            retreatState.allPacks = packs.ToArray();
            if(stateMachine.GetCurrentState() != retreatState)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "I have low health, looking for healthpack");

                stateMachine.SetState(retreatState);
            }
        }
        else //Otherwise hide!
        {
            if(!hideState.isHiding)
            {
                Diagnostic.Instance.AddLog(enemyObj.gameObject, "I have low health, no healthpack around, will hide");

                stateMachine.SetState(hideState);
            }
        }
    }
}
