using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapLoader
{
    private static GameObject mapParent;
    private static GameObject floorParent;
    private static GameObject enemyParent;

    public static void SpawnMap(MapSO loadMap, Action<GameObject, int , int> onTileSpawn = null)
    {

        RegenerateParents();
        for (int x = 0; x < loadMap.width; x++)
        {
            for (int z = 0; z < loadMap.height; z++)
            {
                GameObject tile = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/FloorTile"), new Vector3(x, 0, z), Quaternion.Euler(new Vector3(90, 0, 0)));
                tile.transform.SetParent(floorParent.transform);
                tile.GetComponent<FloorTile>().IsWalkable = loadMap.map[z * loadMap.width + x];

                onTileSpawn?.Invoke(tile, x, z);
            }
        }

        //Enemies
        var enemyFiles = Resources.LoadAll<EnemyPatrolSO>("Maps/Level_" + loadMap.name + "/Enemies");
        for(int i=0; i< enemyFiles.Length; i++)
        {
            if (enemyFiles[i].name.Contains("meta")) continue;

            Vector3 first = enemyFiles[i].patrolPoint[0];
            GameObject enemy = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/Enemy"), first + Vector3.up * 0.6f, Quaternion.Euler(0,enemyFiles[i].rotations[0],0));
            enemy.name = "Enemy_" + i;
            enemy.transform.SetParent(enemyParent.transform);
            //enemy.GetComponent<Enemy>().patrolSO = enemyFiles[i];
        }
    }
    // Used by GenerateMap
    public static void SpawnMap(int width, int height, Action<GameObject, int, int> onTileSpawn = null)
    {

        RegenerateParents();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject tile = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/FloorTile"), new Vector3(x, 0, z), Quaternion.Euler(new Vector3(90, 0, 0)));
                tile.transform.SetParent(floorParent.transform);
                onTileSpawn?.Invoke(tile, x, z);

            }
        }
    }

    static void DestroyGhost()
    {
        GameObject g = GameObject.Find("Ghost");
        if (g)
            UnityEngine.Object.DestroyImmediate(g);
    }

    public static void RegenerateParents()
    {
        DestroyGhost();

        mapParent = GameObject.Find("Map");
        if (mapParent != null)
        {
            UnityEngine.Object.DestroyImmediate(mapParent);
        }

        mapParent = new GameObject("Map");

        floorParent = GameObject.Find("Floor");
        if (floorParent != null)
        {
            UnityEngine.Object.DestroyImmediate(floorParent);
        }

        floorParent = new GameObject("Floor");



        enemyParent = GameObject.Find("Enemies");
        if (enemyParent != null)
        {
            UnityEngine.Object.DestroyImmediate(enemyParent);
        }

        enemyParent = new GameObject("Enemies");

        floorParent.transform.SetParent(mapParent.transform);
        enemyParent.transform.SetParent(mapParent.transform);
    }
}
