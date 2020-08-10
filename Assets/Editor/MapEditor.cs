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
    public static bool active = true;
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
        Enemy
    }
    private bool loadedClicked = false;
    private int controlID;
    private int width;
    private int height;

    private GameObject startTile;
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
        active = true;
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
        if (!active) return;

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

                if(Event.current.type == EventType.MouseDown && Event.current.button == 2)
                {
                    ghost.transform.Rotate(Vector3.up, 90.0f);
                }

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
       

        //On click
        if (Event.current.type == EventType.MouseDown && currentTile != null)
        {
            controlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = controlID;
            if (Event.current.button == 0)
            {
                leftDown = true;
                if (currentTile != null && currentTile.GetComponent<FloorTile>().IsWalkable)
                {
                    if (objectType == (int)PlacebleObjects.Start_Tile)
                    {                     

                        startTile = currentTile;
                    }
                    else if (objectType == (int)PlacebleObjects.Enemy)
                    {
                        Vector3 pos = ghost.transform.position;

                        if (!objectsArray.ContainsKey("Enemy"))
                            objectsArray.Add("Enemy", new List<Vector3>());

                        bool found = objectsArray["Enemy"].Exists((Vector3 v) => { return v.x == pos.x && v.z == pos.z; });
                        if (!found)
                        {
                            GameObject g = (GameObject)Instantiate(Resources.Load("Prefab/Enemy"), new Vector3(currentTile.transform.position.x, 0.6f, currentTile.transform.position.z), ghost.transform.rotation);
                            int count = GameObject.FindGameObjectsWithTag("Enemy").Count() - 1;
                            objectsArray["Enemy"].Add(pos);
                            g.transform.SetParent(GameObject.Find("Enemies").transform);

                            if (!Directory.Exists("Assets/Resources/Maps/Level_" + mapSaveName + "/Enemies"))
                                Directory.CreateDirectory("Assets/Resources/Maps/Level_" + mapSaveName + "/Enemies");

                            /*g.GetComponent<Enemy>().patrolSO = ScriptableObject.CreateInstance<EnemyPatrolSO>();
                            g.GetComponent<Enemy>().patrolSO.patrolPoint = new List<Vector3>();
                            g.GetComponent<Enemy>().patrolSO.times = new List<float>();
                            g.GetComponent<Enemy>().patrolSO.rotations = new List<float>();
                            g.GetComponent<Enemy>().patrolSO.patrolPoint.Add(currentTile.transform.position);
                            g.GetComponent<Enemy>().patrolSO.times.Add(3.0f);
                            g.GetComponent<Enemy>().patrolSO.rotations.Add(ghost.transform.rotation.eulerAngles.y);

                            AssetDatabase.CreateAsset(g.GetComponent<Enemy>().patrolSO, "Assets/Resources/Maps/Level_" + mapSaveName + "/Enemies/Enemy_" + GUID.Generate() + ".asset");
                            EditorUtility.SetDirty(g.GetComponent<Enemy>().patrolSO);*/
                            AssetDatabase.SaveAssets();

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
                        {
                            string path = AssetDatabase.GetAssetPath(e.GetComponent<Enemy>().patrolSO);
                            File.Delete(path);
                            UnityEngine.Object.DestroyImmediate(e);
                            AssetDatabase.Refresh();
                        }
                        
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

        if (startTile == null)
        {
            Debug.LogError("Start tile  required to save map1!");
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

        mapso.enemyCount = enemies.Length;

       /* if(!Directory.Exists(folderPath + "/Enemies"))
            Directory.CreateDirectory(folderPath + "/Enemies");

        List<string> savedFiles = new List<string>();
         if (enemies.Length > 0)
         {
            int counter = 0;
             foreach(GameObject go in enemies)
             {
                //Check for existing SO
                EnemyPatrolSO enemySO = AssetDatabase.LoadAssetAtPath<EnemyPatrolSO>(folderPath + "/Enemies/" + "Enemy_" + counter + ".asset");
                if (enemySO == null)
                {
                    enemySO = ScriptableObject.CreateInstance<EnemyPatrolSO>();
                    enemySO.patrolPoint = new List<Vector3>();
                    enemySO.times = new List<float>();
                    enemySO.patrolPoint.Add(new Vector3(go.transform.position.x, 0.0f, go.transform.position.z));
                    enemySO.times.Add(3);
                    AssetDatabase.CreateAsset(enemySO, folderPath + "/Enemies/" + "Enemy_" + counter + ".asset");
                }
                savedFiles.Add(enemySO.name);
                EditorUtility.SetDirty(enemySO);
                counter++;
                go.GetComponent<Enemy>().patrolSO = enemySO;
             }          
        }

        //Get all files in enemy folder
        string[] files = Directory.GetFiles(folderPath + "/Enemies");
        foreach (string file in files)
        {
            if (!savedFiles.Contains<string>(file))
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }*/


        EditorUtility.SetDirty(mapso);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadMap()
    {
        mapSaveName = loadMap.name;
        objectsArray.Clear();


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

        if (!objectsArray.ContainsKey("Enemy"))
            objectsArray.Add("Enemy", new List<Vector3>());

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in enemies)
        {
            objectsArray["Enemy"].Add(go.transform.position);
        }

    }



    private void GenerateMap()
    {
        MapLoader.RegenerateParents();
        objectsArray.Clear();
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
