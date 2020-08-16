using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootState : EnemyState
{
    float shootRate;
    float stoppingDistance = 5.0f;
    public ShootState(Personality e, float shootRate) : base(e)
    {
        this.shootRate = shootRate;
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\shoot");

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
        personalityObj.enemyObj.rifle.Shoot(shootRate, personalityObj.enemyObj.gameObject, personalityObj.enemyObj.damageGiven);

    }
    public override void OnExit()
    {

    }
}
