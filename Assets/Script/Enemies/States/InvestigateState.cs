using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : EnemyState
{
    public Vector3 investigationPoint;
    public bool done = false;
    public float timer;
    public float rotateTimer = 0;
    public InvestigateState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\investigate");
    }


    public override void OnEnter()
    {
        base.OnEnter();
        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {
            personalityObj.enemyObj.StartCoroutine(RotateAround());

        });
        personalityObj.enemyObj.StopAllCoroutines();
        personalityObj.enemyObj.enemySight.StartLookingForPlayer();
        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(investigationPoint);
        timer = 0;
        done = false;

    }

    IEnumerator RotateAround()
    {
        yield return new WaitForSeconds(2);
        rotateTimer = Random.Range(1,3);
        while (rotateTimer > 0)
        {
            rotateTimer -= Time.deltaTime;
            personalityObj.enemyObj.transform.Rotate(Vector3.up, 20 * Random.Range(0, 2) == 1 ? 1 : -1);
            yield return null;

        }
        yield return new WaitForSeconds(3);

        rotateTimer = Random.Range(1,3);
        while (rotateTimer > 0)
        {
            rotateTimer -= Time.deltaTime;
            personalityObj.enemyObj.transform.Rotate(Vector3.up, 30 * Random.Range(0, 2) == 1 ? 1 : -1);
            yield return null;

        }

        rotateTimer = 0;
        done = true;
    }
    public override void Update()
    {
        base.Update();



  

    }
    public override void OnExit()
    {
        Debug.Log("Exit shoot");

    }
}
