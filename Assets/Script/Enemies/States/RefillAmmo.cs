using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RefifllAmmo : EnemyState
{
    Sprite hideSprite;

    public Vector3? playerLastKnowsPosition;
    public GameObject[] allPacks;

    public RefifllAmmo(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\ammo");
        stateName = "RefillAmmo";
    }


    IEnumerator Go(Vector3 pos)
    {
        yield return new WaitForSeconds(0.2f);

        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(pos);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Vector3? closestPack = null;
        float closestDist = 10000000.0f;
        personalityObj.enemyObj.StopAllCoroutines();

        //For some reason, sometimes is null
        if (allPacks == null)
            allPacks = GameManager.Instance.GetAvailableAmmoPacks().ToArray();
        foreach (GameObject pack in allPacks)
        {
            if (pack.GetComponent<AmmoBox>().isAvailable)
            {
                float dist = (personalityObj.enemyObj.transform.position - pack.transform.position).magnitude;
                if (dist < closestDist)
                {
                    closestPack = pack.transform.position;
                    closestDist = dist;
                }
            }
        }

        personalityObj.enemyObj.StartCoroutine(Go(closestPack.Value));

    }

    public override void Update()
    {
        base.Update();


    }
    public override void OnExit()
    {
        personalityObj.enemyObj.StopAllCoroutines();

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() => {

        });
    }


}
