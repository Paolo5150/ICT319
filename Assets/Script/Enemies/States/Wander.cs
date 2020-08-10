using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : EnemyState<Beserk>
{ 
    public Vector3 randomPos;

    private bool isLookingForSpot = false;
    public Wander(Beserk personality) : base(personality)
    {
    }

    IEnumerator LookForRandomSpot()
    {
        isLookingForSpot = true;
        yield return new WaitForSeconds(5);
        Vector3 randomDirection = Random.insideUnitSphere * 100;
        NavMesh.SamplePosition(enemyObj.transform.position + randomDirection, out NavMeshHit hit, 100, 1);
        randomPos = hit.position;
        enemyObj.navigator.Go(randomPos);
        isLookingForSpot = false;

    }
    public override void OnEnter()
    {
        enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {
            if (!isLookingForSpot && !enemyObj.navigator.isMoving)
                enemyObj.StartCoroutine(LookForRandomSpot());
        });

        enemyObj.StartCoroutine(LookForRandomSpot());


    }
    public override void Update()
    {

    }
    public override void OnExit()
    {
    }
}
