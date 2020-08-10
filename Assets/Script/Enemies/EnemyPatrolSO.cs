using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolSO : ScriptableObject
{
    [SerializeField]
    public List<Vector3> patrolPoint;

    [SerializeField]
    public List<float> times;

    [SerializeField]
    public List<float> rotations;
}
