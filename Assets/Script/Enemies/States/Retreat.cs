using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Retreat : EnemyState
{
    Sprite hideSprite;
    public float minHealthForRetreat;

    public Vector3? playerLastKnowsPosition;
    public GameObject[] allPacks;

    public Retreat(Personality e, float minHealthRetreat) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\retreat");
        minHealthForRetreat = minHealthRetreat;
        stateName = "Retreat";
    }

    IEnumerator Go(Vector3 dest)
    {
        yield return new WaitForSeconds(0.5f);
        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(dest);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() => {

        });

        Vector3? closestPack = null;
        if (allPacks == null)
            allPacks = GameManager.Instance.GetAvailableHealthPacks().ToArray();

        personalityObj.enemyObj.StopAllCoroutines();

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

       // personalityObj.enemyObj.StartCoroutine(Go(closestPack));
        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(closestPack.Value);

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
