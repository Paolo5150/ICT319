using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public EnemyPatrolSO patrolSO;
    [HideInInspector]
    public Navigator navigator;


    // Start is called before the first frame update
    public virtual void Start()
    {
        navigator = GetComponent<Navigator>();
        navigator.Init();
    }


}
