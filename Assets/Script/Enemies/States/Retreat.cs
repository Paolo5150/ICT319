using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Retreat : EnemyState
{
    Sprite hideSprite;
    float minHealthForRetreat;

    public Vector3? playerLastKnowsPosition;
    public Vector3 closestPack;
    public Retreat(Personality e, float minHealthRetreat) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\retreat");
        minHealthForRetreat = minHealthRetreat;
        stateName = "Retreat";
    }

    public override void OnEnter()
    {
        base.OnEnter();
 
        GoGetHealthPack();
    }

    private void GoGetHealthPack()
    {       

        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(closestPack);
    }

    public override void Update()
    {
        base.Update();
       

    }
    public override void OnExit()
    {

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() => {

        });
    }

  
}
