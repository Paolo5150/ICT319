using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    public string mapLoad = "1";

    private  MapSO currentMap;
    private NavMeshSurface navMeshSurface;
    private GameObject floorParent;
    private GameObject mapParent;

    public GameObject menuPanel;
    public int beserkCount = 1;
    public int soldierCount = 1;
    public int cowardCount = 1;

    public int healthPacksCount = 1;
    public int ammoboxCount = 1;

    public InputField cowardField;
    public InputField soldierField;
    public InputField beserkField;

    public InputField healthPackField;
    public InputField ammoboxField;


    public GameObject[] beserks;
    public GameObject[] soldiers;
    public GameObject[] cowards;
    public GameObject[] healthPacks;
    public GameObject[] ammoBoxes;

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

    // Start is called before the first frame update
    void Start()
    {
        obstacles = new List<Vector3>();
        navMeshSurface = GetComponent<NavMeshSurface>();

        for (int i = 0; i < 3; i++)
        {
            beserks[i].SetActive(false);
            cowards[i].SetActive(false);
            soldiers[i].SetActive(false);
        }
        //   LoadMap();

        beserkField.text = "0";
        cowardField.text = "0";
        soldierField.text = "0";
        healthPackField.text = "1";
        ammoboxField.text = "1";

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Restart();
    } 

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public List<GameObject> GetAvailableAmmoPacks()
    {
        List<GameObject> available = new List<GameObject>();

        GameObject[] packs = GameObject.FindGameObjectsWithTag("AmmoBox");
        foreach(GameObject g in packs)
        {
            if (g.GetComponent<AmmoBox>().isAvailable)
                available.Add(g);
        }

        return available;
    }

    public List<GameObject> GetAvailableHealthPacks()
    {
        List<GameObject> available = new List<GameObject>();

        GameObject[] packs = GameObject.FindGameObjectsWithTag("Healthpack");
        foreach (GameObject g in packs)
        {
            if (g.GetComponent<Healthpack>().isAvailable)
                available.Add(g);
        }

        return available;
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
        Player.Instance.Init();

        MainCanvas.Instance.Init();
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        navMeshSurface.BuildNavMesh();


        beserkCount = int.Parse(beserkField.text);
        cowardCount = int.Parse(cowardField.text);
        soldierCount = int.Parse(soldierField.text);

        healthPacksCount = int.Parse(healthPackField.text);
        ammoboxCount = int.Parse(ammoboxField.text);

        beserkCount = Mathf.Clamp(beserkCount, 0, 3);
        soldierCount = Mathf.Clamp(soldierCount, 0, 3);
        cowardCount = Mathf.Clamp(cowardCount, 0, 3);
        healthPacksCount = Mathf.Clamp(healthPacksCount, 0, 3);
        ammoboxCount= Mathf.Clamp(ammoboxCount, 0, 3);


        List<GameObject> activeEnemies = new List<GameObject>();

        for(int i=0; i< 3; i++)
        {
            bool val;
            val = i < beserkCount;
            beserks[i].SetActive(val);
            if (val)
                activeEnemies.Add(beserks[i]);

            val = i < soldierCount;
            soldiers[i].SetActive(val);
            if (val)
                activeEnemies.Add(soldiers[i]);

            val = i < cowardCount;
            cowards[i].SetActive(val);
            if (val)
                activeEnemies.Add(cowards[i]);
        }

        foreach(GameObject g in activeEnemies)
            g.GetComponent<Enemy>().Init();

        GameObject[] allHP = GameObject.FindGameObjectsWithTag("Healthpack");
        GameObject[] allAB = GameObject.FindGameObjectsWithTag("AmmoBox");
        for(int i = 0; i< 3; i++)
        {
            if (i < ammoboxCount)
                allAB[i].SetActive(true);
            else
                allAB[i].SetActive(false);

            if (i < healthPacksCount)
                allHP[i].SetActive(true);
            else
                allHP[i].SetActive(false);

        }

        menuPanel.SetActive(false);
    }


}
