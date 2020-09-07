using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StunnedState : EnemyState
{
    public bool isStunned = false;
    Vector3 playerLastKnownPosition;

    float stunTimer = 0f;

    float originalSight;
    float originalHear;
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
        originalSight =  personalityObj.enemyObj.enemySight.viewRange;
        originalHear = personalityObj.enemyObj.enemySight.hearRange;
        personalityObj.enemyObj.enemySight.viewRange = 0;
        personalityObj.enemyObj.enemySight.hearRange = 0;
        isStunned = true;
        Debug.Log("Enter stunned");
    }

    public override void Update()
    {
        base.Update();

        if(isStunned)
        {
            stunTimer += Time.deltaTime;
            personalityObj.enemyObj.transform.Rotate(Vector3.up, 5.0f);
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
        stunTimer = 0;
        personalityObj.enemyObj.enemySight.viewRange = originalSight;
        personalityObj.enemyObj.enemySight.hearRange = originalHear;


    }
}
