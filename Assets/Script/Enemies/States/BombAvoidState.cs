using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombAvoid : EnemyState
{
    public bool readyToGo = false;

    public Vector3 lastBombPosition;

    EnemyState previousState;
    public BombAvoid(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\what");
    }

    IEnumerator FindRoute()
    {
        yield return new WaitForSeconds(2.0f);
        readyToGo = true;
        yield return new WaitForSeconds(1.0f);

        GameObject.FindGameObjectWithTag("Bomb").GetComponent<NavMeshObstacle>().carving = false;

    }

    public override void OnEnter()
    {
        Debug.Log("Enter bomb avoid");
        readyToGo = false;
        // base.OnEnter();
        if(!readyToGo)
        {

            personalityObj.enemyObj.enemySight.StopLookingForPlayer();
            personalityObj.enemyObj.navigator.Stop();
            GameObject.FindGameObjectWithTag("Bomb").GetComponent<NavMeshObstacle>().carving = true;
            personalityObj.enemyObj.StartCoroutine(FindRoute());
        }


    }

    public override void Update()
    {
        base.Update();




    }
    public override void OnExit()
    {
        Debug.Log("exit bomb avoid");

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() =>
        {

        });
    }
}
