using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class MapEditor : EditorWindow
{
   
    private static MapEditor instance;
    public static MapEditor Instance
    {
        get
        {
            if (instance == null)
                instance = new MapEditor();

            return instance;
        }
    }
    private enum PlacebleObjects
    {
        Tile,
        Start_Tile,
        End_Tile,
        Enemy
    }
    private bool loadedClicked = false;
    private int controlID;
    private int width;
    private int height;

    private GameObject startTile;
    private GameObject endTile;
    private GameObject currentTile;
    private int objectType = 0;
    private GameObject[,] mapArray;

    private Dictionary<string, List<Vector3>> objectsArray = new Dictionary<string, List<Vector3>>();

    private GameObject ghost;

    private MapSO loadMap;
    private string mapSaveName = "";

    private bool leftDown = false;
    private bool rightDown = false;

    private bool[] foldouts = new bool[20];

    enum FoldoutsName
    {
        QUICK_ACTIONS,
    }

    [MenuItem("Window/My Map Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        window.Show();
    }

    public void OnInspectorUpdate()
    {       

        this.Repaint();
    }


    // Called to draw the MapEditor windows.
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
       
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Generate map"))
        {
            GenerateMap();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Place tile");
        objectType = EditorGUILayout.Popup(objectType, Enum.GetNames(typeof(PlacebleObjects)));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save map"))
        {
            if(mapSaveName != "")
                SaveMap();
        }
        mapSaveName = EditorGUILayout.TextField(mapSaveName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load map"))
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<MapSO>(loadMap, false, "", controlID);
            loadedClicked = true;
        }
        
        EditorGUILayout.EndHorizontal();


        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            loadMap = (MapSO)EditorGUIUtility.GetObjectPickerObject();
            if (loadMap != null && loadedClicked)
            {
                loadedClicked = false;

                LoadMap();
            }
        }

        foldouts[(int)FoldoutsName.QUICK_ACTIONS] = EditorGUILayout.Foldout(foldouts[(int)FoldoutsName.QUICK_ACTIONS], "Actions");
        if (foldouts[(int)FoldoutsName.QUICK_ACTIONS])
        {
            if (GUILayout.Button("Flatten"))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        mapArray[x, z].GetComponent<FloorTile>().IsWalkable = true;
                    }
                }
            }
            if (GUILayout.Button("Outer wall"))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                            mapArray[x, z].GetComponent<FloorTile>().IsWalkable = false;
                    }
                }
            }
        }
    }


    // Does the rendering of the map editor in the scene view.
    private void OnSceneGUI(SceneView sceneView)
    {
        Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        int floorTileLayer = LayerMask.GetMask("FloorTile");

        if (Physics.Raycast(r,out hit, 1000, floorTileLayer))
        {
            currentTile = hit.transform.gameObject;

            if (objectType == (int)PlacebleObjects.Enemy)
            {
                if (ghost == null)
                {
                    ghost = (GameObject)Instantiate(Resources.Load("Prefab/Enemy"), new Vector3(currentTile.transform.position.x, 0.6f, currentTile.transform.position.z), Quaternion.identity);
                    ghost.name = "Ghost";
                    ghost.tag = "Untagged";
                }
                if(currentTile.GetComponent<FloorTile>().IsWalkable)
                    ghost.transform.position = new Vector3(currentTile.transform.position.x, 0.6f, currentTile.transform.position.z);
                else
                    ghost.transform.position = new Vector3(-10000.0f, -10000.0f, -10000.0f); // just off screen

            }
            else
            {
                if(ghost != null)
                    UnityEngine.Object.DestroyImmediate(ghost);

                ghost = null;
            }

        }
        else
        {
            currentTile = null;         
            DestroyGhost();
        }

        //Gizmos
        if (currentTile)
            DrawTileBorders(currentTile.transform.position, Color.blue);
        if(startTile)
            DrawTileBorders(startTile.transform.position, Color.green);
        if (endTile)
            DrawTileBorders(endTile.transform.position, Color.red);

        

        //On click
        if (Event.current.type == EventType.MouseDown && currentTile != null)
        {
            controlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = controlID;
            if (Event.current.button == 0)
            {
                leftDown = true;
                if (currentTile != null)
                {
                    if (objectType == (int)PlacebleObjects.Start_Tile)
                    {                     

                        startTile = currentTile;
                    }
                    else if (objectType == (int)PlacebleObjects.End_Tile)
                    {                      
                        endTile = currentTile;
                    }
                    else if (objectType == (int)PlacebleObjects.Enemy)
                    {
                        Vector3 pos = ghost.transform.position;

                        if (!objectsArray.ContainsKey("Enemy"))
                            objectsArray.Add("Enemy", new List<Vector3>());

                        bool found = objectsArray["Enemy"].Exists((Vector3 v) => { return v.x == pos.x && v.z == pos.z; });
                        if (!found)
                        {
                            GameObject g = (GameObject)Instantiate(Resources.Load("Prefab/Enemy"), new Vector3(currentTile.transform.position.x, 0.6f, currentTile.transform.position.z), Quaternion.identity);
                            int count = GameObject.FindGameObjectsWithTag("Enemy").Count();
                            g.name = "Enemy " + count;
                            g.GetComponent<Enemy>().patrolSO = ScriptableObject.CreateInstance<EnemyPatrolSO>();
                            g.GetComponent<Enemy>().patrolSO.patrolPoint.Add(currentTile.transform.position);
                            objectsArray["Enemy"].Add(pos);
                            g.transform.SetParent(GameObject.Find("Enemies").transform);
                            
                        }

                    }
                }

            }
            else if (Event.current.button == 1)
            {
                rightDown = true;               
            }
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (Event.current.button == 0)
                leftDown = false;
            else if (Event.current.button == 1)
                rightDown = false;

            if (GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;
            }
        }

        if(leftDown)
        {
            if(currentTile != null)
            {
                if (objectType == (int)PlacebleObjects.Tile)
                    currentTile.GetComponent<FloorTile>().IsWalkable = true;
               
            }
        }

        if(rightDown)
        {
            if (currentTile != null)
            {
                if (objectType == (int)PlacebleObjects.Tile)
                    currentTile.GetComponent<FloorTile>().IsWalkable = false;
                else if (objectType == (int)PlacebleObjects.Enemy)
                {
                    Vector3 pos = ghost.transform.position;

                    if (!objectsArray.ContainsKey("Enemy"))
                        objectsArray.Add("Enemy", new List<Vector3>());

                    bool found = objectsArray["Enemy"].Exists((Vector3 v) => { return v.x == pos.x && v.z == pos.z; });
                    if (found)
                    {
                        objectsArray["Enemy"].Remove(pos);
                        var enemy = GameObject.FindGameObjectsWithTag("Enemy").Where((GameObject g)=> { return g.transform.position.x == pos.x && g.transform.position.z == pos.z; });
                        foreach (GameObject e in enemy)
                            UnityEngine.Object.DestroyImmediate(e);
                        
                    }

                }
            }
        }

    }

 

    void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI; // Just in case
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

 
    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void DestroyGhost()
    {
        GameObject g = GameObject.Find("Ghost");
        if (g)
            UnityEngine.Object.DestroyImmediate(g);
    }

    private void SaveMap()
    {

        if (startTile == null || endTile == null)
        {
            Debug.LogError("Start tile and end tile required to save map1!");
            return;
        }

        string folderName = "Level_" + mapSaveName;
        string folderPath = "Assets/Resources/Maps/" + folderName;
        string mapName = folderPath + "/" + mapSaveName + ".asset";

        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Resources/Maps", folderName);

        MapSO mapso = AssetDatabase.LoadAssetAtPath<MapSO>(mapName);

        if (mapso == null)
        {
            mapso = ScriptableObject.CreateInstance<MapSO>();
            AssetDatabase.CreateAsset(mapso, mapName);
        }

        mapso.endTile = endTile.transform.position;
        mapso.startTile = startTile.transform.position;
        mapso.width = width;
        mapso.height = height;
        mapso.map = new bool[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                mapso.map[z * height + x] = mapArray[x, z].GetComponent<FloorTile>().IsWalkable;
            }
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        /* if(enemies.Length > 0)
         {
             mapso.enemies = new EnemyPatrolSO[enemies.Length];
             int counter = 0;
             foreach(GameObject go in enemies)
             {
                 mapso.enemies[counter] = ScriptableObject.CreateInstance<EnemyPatrolSO>();
                 mapso.enemies[counter].patrolPoint = new Vector3[go.GetComponent<Enemy>().patrolSO.patrolPoint.Length];
                 for(int i=0; i< go.GetComponent<Enemy>().patrolSO.patrolPoint.Length;i++)
                 {
                     mapso.enemies[counter].patrolPoint[i] = go.GetComponent<Enemy>().patrolSO.patrolPoint[i];
                 }
                 counter++;
             }
         }*/
        EditorUtility.SetDirty(mapso);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadMap()
    {
        mapSaveName = loadMap.name;

        string folderName = "Level_" + mapSaveName;
        string folderPath = "Assets/Resources/Maps/" + folderName;
        string mapName = folderPath + "/" + mapSaveName + ".asset";

        loadMap = AssetDatabase.LoadAssetAtPath<MapSO>(mapName);
        width = loadMap.width;
        height = loadMap.height;
        mapArray = new GameObject[loadMap.width, loadMap.height];
        MapLoader.SpawnMap(loadMap, (GameObject tile, int x, int z) => {
            mapArray[x, z] = tile;
        });       
        startTile = mapArray[(int)loadMap.startTile.x, (int)loadMap.startTile.z].gameObject;
        endTile = mapArray[(int)loadMap.endTile.x, (int)loadMap.endTile.z].gameObject;
        
    }

  

    private void GenerateMap()
    {
        MapLoader.RegenerateParents();

        mapSaveName = "";
        loadMap = null;
        mapArray = new GameObject[width, height];
        MapLoader.SpawnMap(width, height, (GameObject tile, int x, int z) => {
            mapArray[x, z] = tile;
        });
    }

    void DrawTileBorders(Vector3 center, Color color)
    {
        Handles.color = color;
        Handles.DrawLine(new Vector3(center.x - 0.5f, 0.0f, center.z - 0.5f), new Vector3(center.x + 0.5f, 0.0f, center.z - 0.5f));
        Handles.DrawLine(new Vector3(center.x + 0.5f, 0.0f, center.z - 0.5f), new Vector3(center.x + 0.5f, 0.0f, center.z + 0.5f));
        Handles.DrawLine(new Vector3(center.x + 0.5f, 0.0f, center.z + 0.5f), new Vector3(center.x - 0.5f, 0.0f, center.z + 0.5f));
        Handles.DrawLine(new Vector3(center.x - 0.5f, 0.0f, center.z + 0.5f), new Vector3(center.x - 0.5f, 0.0f, center.z - 0.5f));
    }
}
