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

    }

    IEnumerator LookForRandomSpot()
    {
        isLookingForSpot = true;
        Debug.Log("Looking for random spot and going there");
        yield return new WaitForSeconds(2);
        Vector3 randomDirection = Random.insideUnitSphere * 100;
        NavMesh.SamplePosition(personalityObj.enemyObj.transform.position + randomDirection, out NavMeshHit hit, 100, 1);
        randomPos = hit.position;
        personalityObj.enemyObj.navigator.Go(randomPos);
        isLookingForSpot = false;

    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Enter wander");
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
        personalityObj.enemyObj.StopAllCoroutines();
        personalityObj.enemyObj.navigator.Stop();

        Debug.Log("OnExit wander");

    }
}
