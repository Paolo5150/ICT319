using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    private List<Vector3> obstacles;
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

    public List<Vector3> GetObstacles()
    {
        return obstacles;
    }

    public Vector2 GetCurrentMapEndTilePos()
    {
        return endTileVec2Pos;
    }
    // Start is called before the first frame update
    void Start()
    {
        Player.Instance.Init();
        obstacles = new List<Vector3>();
        navMeshSurface = GetComponent<NavMeshSurface>();
        LoadMap();

    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RecreateNavPath()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void LoadMap()
    {
        currentMap = (MapSO)Resources.Load("Maps\\Level_" + mapLoad + "\\" + mapLoad);
        if(currentMap != null)
        {
            MapLoader.SpawnMap(currentMap,(GameObject tile, int x, int z)=> {

                if (!tile.GetComponent<FloorTile>().IsWalkable)
                {
                    if(tile.transform.position.x != 0 && tile.transform.position.x != currentMap.width - 1 &&
                    tile.transform.position.z != 0 && tile.transform.position.z != currentMap.height - 1)
                        obstacles.Add(tile.transform.position);
                }

                if (x == currentMap.endTile.x && z == currentMap.endTile.z)
                    tile.GetComponent<MeshRenderer>().material.color = Color.green;
            });

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(currentMap.startTile.x, 0.6f, currentMap.startTile.z);
           // endTileVec2Pos = new Vector2(currentMap.endTile.x, currentMap.endTile.z);
        }

        MainCanvas.Instance.Init();
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject g in allEnemies)
            g.GetComponent<Enemy>().Init();

            navMeshSurface.BuildNavMesh();
    }


}
