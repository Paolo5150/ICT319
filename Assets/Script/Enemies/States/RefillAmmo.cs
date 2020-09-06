using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RefifllAmmo : EnemyState
{
    Sprite hideSprite;

    public Vector3? playerLastKnowsPosition;
    public GameObject[] allPacks;

    GameObject targetPack = null;

    float timer = 1.0f;
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
        GameObject closestPack = null;
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
                    closestPack = pack;
                    closestDist = dist;
                }
            }
        }
        targetPack = closestPack;
        personalityObj.enemyObj.StartCoroutine(Go(closestPack.transform.position));

    }

    public override void Update()
    {
        base.Update();

        if(timer <= 0)
        {
            timer = 1.0f;
            float closestDist = 10000000.0f;
            allPacks = GameManager.Instance.GetAvailableAmmoPacks().ToArray();
            GameObject closestPack = null;

            foreach (GameObject pack in allPacks)
            {
                if (pack.GetComponent<AmmoBox>().isAvailable)
                {
                    float dist = (personalityObj.enemyObj.transform.position - pack.transform.position).magnitude;
                    if (dist < closestDist)
                    {
                        closestPack = pack;
                        closestDist = dist;
                    }
                }
            }

            if(closestPack != targetPack)
            {
                targetPack = closestPack;
                personalityObj.enemyObj.StartCoroutine(Go(closestPack.transform.position));
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }


    }
    public override void OnExit()
    {
        personalityObj.enemyObj.StopAllCoroutines();

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() => {

        });
    }


}
