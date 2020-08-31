using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : EnemyState
{
    public Vector3? playerLastKnownPosition;
    float stoppingDistance = 1.5f;

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
                personalityObj.enemyObj.navigator.Stop();
            else
                personalityObj.enemyObj.navigator.Go(Player.Instance.transform.position);
        }
        else
            playerLastKnownPosition = Player.Instance.transform.position;




    }
    public override void OnExit()
    {
        animator.SetBool("Attack", false);

    }
}
