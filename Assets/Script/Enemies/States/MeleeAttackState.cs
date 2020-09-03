using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : EnemyState
{
    public Vector3? playerLastKnownPosition;
    float stoppingDistance = 1.5f;

    float meleeDamage = 5.0f;
    float hitRate = 0.8f;
    float timer = 0;
    Animator animator;
    public MeleeAttackState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\shoot");
        stateName = "MeleeAttack";
        animator = personalityObj.enemyObj.GetComponentInChildren<Animator>();
    }


    public override void OnEnter()
    {

        base.OnEnter();
        animator.SetBool("Attack",true);

    }
    public override void Update()
    {
        base.Update();

        personalityObj.enemyObj.transform.LookAt(Player.Instance.transform.position);

        if (personalityObj.enemyObj.enemySight.IsPlayerInSight())
        {
            playerLastKnownPosition = null;
            float toPlayerDist = (Player.Instance.transform.position - personalityObj.enemyObj.transform.position).magnitude;
            if (toPlayerDist < stoppingDistance) // Stopping distance, should be lower than view range
            {
                personalityObj.enemyObj.navigator.Stop();
                if (timer <= 0)
                {
                    Player.Instance.OnGetShot(personalityObj.enemyObj.gameObject, meleeDamage);
                    timer = hitRate;
                }
                else
                    timer -= Time.deltaTime;

            }
            else
            {
                personalityObj.enemyObj.navigator.Go(Player.Instance.transform.position);
                timer = hitRate;
            }
        }
        else
            playerLastKnownPosition = Player.Instance.transform.position;




    }
    public override void OnExit()
    {
        animator.SetBool("Attack", false);
        timer = 0;

    }
}
