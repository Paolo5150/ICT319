﻿using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    public string mapLoad = "1";

    private  MapSO currentMap;
    private NavMeshSurface navMeshSurface;
    private GameObject floorParent;
    private GameObject mapParent;
    private Vector2 endTileVec2Pos;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Vector2 GetCurrentMapEndTilePos()
    {
        return endTileVec2Pos;
    }
    // Start is called before the first frame update
    void Start()
    {
        Player.Instance.Init();
        navMeshSurface = GetComponent<NavMeshSurface>();
        LoadMap();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator TestLoading()
    {
        for(int i=0; i< 10; i++)
        {
            if (mapLoad.Equals("2"))
                mapLoad = "1";
            else
                mapLoad = "2";

            LoadMap();
            yield return new WaitForSeconds(5);
        }
    }

    


    public void LoadMap()
    {
        currentMap = (MapSO)Resources.Load("Maps\\Level_" + mapLoad + "\\" + mapLoad);
        if(currentMap != null)
        {
            MapLoader.SpawnMap(currentMap);

            Player.Instance.transform.position = new Vector3(currentMap.startTile.x, 0.6f, currentMap.startTile.z);
            endTileVec2Pos = new Vector2(currentMap.endTile.x, currentMap.endTile.z);
            navMeshSurface.BuildNavMesh();
        }
    }

    public void LoadMap(string value)
    {
        currentMap = (MapSO)Resources.Load("Maps\\Level_" + value + "\\" + mapLoad);
        if (currentMap != null)
        {
            MapLoader.SpawnMap(currentMap);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(currentMap.startTile.x, 0.6f, currentMap.startTile.z);
            endTileVec2Pos = new Vector2(currentMap.endTile.x, currentMap.endTile.z);
            navMeshSurface.BuildNavMesh();
        }
    }


}