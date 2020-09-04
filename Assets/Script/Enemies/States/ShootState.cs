using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootState : EnemyState
{
    float shootRate;
    float stoppingDistance = 5.0f;

    float timer = 0.0f;
    public ShootState(Personality e, float shootRate) : base(e)
    {
        this.shootRate = shootRate;
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\shoot");
        stateName = "Shooting";

    }


    public override void OnEnter()
    {
        base.OnEnter();

    }
    public override void Update()
    {
        base.Update();

        personalityObj.enemyObj.transform.LookAt(Player.Instance.transform.position);

        float toPlayerDist = (Player.Instance.transform.position - personalityObj.enemyObj.transform.position).magnitude;
        if (toPlayerDist < stoppingDistance) // Stopping distance, should be lower than view range
            personalityObj.enemyObj.navigator.Stop();

        if (timer >= shootRate)
        {
            personalityObj.enemyObj.rifle.Shoot(shootRate, personalityObj.enemyObj.gameObject, personalityObj.enemyObj.damageGiven);
            timer = 0.0f;
        }
        else
            timer += Time.deltaTime;
    }
    public override void OnExit()
    {

    }
}
