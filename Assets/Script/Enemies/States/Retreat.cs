using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Retreat : EnemyState
{
    Sprite hideSprite;
    float minHealthForRetreat;

    public Vector3? playerLastKnowsPosition;
    public GameObject[] allPacks;

    public Retreat(Personality e, float minHealthRetreat) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\retreat");
        minHealthForRetreat = minHealthRetreat;
        stateName = "Retreat";
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Vector3 closestPack = new Vector3(0,0,0);
        float closestDist = 10000000.0f;
        foreach (GameObject pack in allPacks)
        {
            if (pack.GetComponent<Healthpack>().isAvailable)
            {
                float dist = (personalityObj.enemyObj.transform.position - pack.transform.position).magnitude;
                if (dist < closestDist)
                {
                    closestPack = pack.transform.position;
                    closestDist = dist;
                }
            }
        }


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
