using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState<T> : State where T : Enemy
{
    public EnemyState(T e)
    {
        enemyObj = e;
    }

    public T enemyObj;
    public override abstract void OnEnter();
    public override abstract void Update();
    public override abstract void OnExit();
}
