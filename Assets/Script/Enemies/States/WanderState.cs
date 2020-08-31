using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : EnemyState
{
    public Vector3 randomPos;

    private bool isLookingForSpot = false;
    public WanderState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\wander");
        stateName = "Wander";

    }

    IEnumerator LookForRandomSpot()
    {
        isLookingForSpot = true;
        yield return new WaitForSeconds(2);
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(10, 60);
        NavMesh.SamplePosition(personalityObj.enemyObj.transform.position + randomDirection, out NavMeshHit hit, Random.Range(10,60), 1);
        randomPos = hit.position;
        personalityObj.enemyObj.navigator.Go(randomPos);
        isLookingForSpot = false;

    }
    public override void OnEnter()
    {
        base.OnEnter();
        isLookingForSpot = false;
        personalityObj.enemyObj.navigator.UseWalkSpeed();
        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {
            if (!isLookingForSpot && !personalityObj.enemyObj.navigator.isMoving)
                personalityObj.enemyObj.StartCoroutine(LookForRandomSpot());
        });
        personalityObj.enemyObj.StopAllCoroutines();
        personalityObj.enemyObj.StartCoroutine(LookForRandomSpot());
        personalityObj.enemyObj.enemySight.StartLookingForPlayer();

    }
    public override void Update()
    {
        base.Update();

    }
    public override void OnExit()
    {
        base.OnExit();
        personalityObj.enemyObj.StopCoroutine(LookForRandomSpot());
        personalityObj.enemyObj.navigator.Stop();
        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {

        });


    }
}
