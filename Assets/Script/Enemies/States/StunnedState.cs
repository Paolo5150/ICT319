using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StunnedState : EnemyState
{
    public bool isStunned = false;

    float stunTimer = 0f;
    public StunnedState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\stunned");
        stateName = "Stunned";

    }



    public override void OnEnter()
    {

        base.OnEnter();
        personalityObj.enemyObj.navigator.Stop();
        personalityObj.enemyObj.enemySight.enabled = false;
        isStunned = true;
        Debug.Log("Enter stunned");
    }

    public override void Update()
    {
        base.Update();

        if(isStunned)
        {
            stunTimer += Time.deltaTime;
            if(stunTimer > 4.0)
            {
                stunTimer = 0.0f;
                isStunned = false;
            }
        }

    }
    public override void OnExit()
    {
        Debug.Log("Exit stunned");

        personalityObj.enemyObj.enemySight.enabled = true;


    }
}
