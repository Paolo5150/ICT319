using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootState : EnemyState
{
    float shootRate;
    public ShootState(Personality e, float shootRate) : base(e)
    {
        this.shootRate = shootRate;
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\shoot");

    }


    public override void OnEnter()
    {
        base.OnEnter();

        Debug.Log("Enter shoot");

    }
    public override void Update()
    {
        base.Update();
        personalityObj.enemyObj.rifle.Shoot(shootRate, personalityObj.enemyObj.gameObject, personalityObj.enemyObj.damageGiven);
        personalityObj.enemyObj.transform.LookAt(Player.Instance.transform.position);

    }
    public override void OnExit()
    {
        Debug.Log("Exit shoot");

    }
}
