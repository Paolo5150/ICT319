using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSO : ScriptableObject
{
    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    [SerializeField]
    public Vector3 startTile;


    [SerializeField]
    public Vector3 endTile;
    [SerializeField]
    public bool[] map;

    [SerializeField]
    public int enemyCount;

}
