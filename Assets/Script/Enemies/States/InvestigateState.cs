using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : EnemyState
{
    public Vector3 investigationPoint;
    public bool done = false;
    private float rotateTimer = 0;
    public bool isInvestigating = false;
    public float waitBeforeGoingToPoint = 0.0f;

    float investigationTimer;

    public InvestigateState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\what");
        stateName = "Investigate";

    }

    IEnumerator StartInvestigating()
    {

        yield return new WaitForSeconds(waitBeforeGoingToPoint);

        personalityObj.enemyObj.navigator.UseRunSpeed();
        personalityObj.enemyObj.navigator.Go(investigationPoint);
    }


    public override void OnEnter()
    {
        base.OnEnter();
        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {
            personalityObj.enemyObj.StartCoroutine(RotateAround());

        });
        personalityObj.enemyObj.enemySight.StartLookingForPlayer();

        personalityObj.enemyObj.transform.LookAt(investigationPoint);

        isInvestigating = true; //Started investigating. Set false after a few seconds, so if another noie is heard the enemy will go check the new noise
        done = false;

        investigationTimer = 0;

        personalityObj.enemyObj.StopAllCoroutines();
        personalityObj.enemyObj.StartCoroutine(StartInvestigating());


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
        yield return new WaitForSeconds(2);

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

        if (investigationTimer < 1.5f && isInvestigating)
            investigationTimer += Time.deltaTime;
        else
            isInvestigating = false;
  

    }
    public override void OnExit()
    {
        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {

        });
    }
}
