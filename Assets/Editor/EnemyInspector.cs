using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyInspector : Editor
{
    Enemy enemyScript;
    Ray r;
    RaycastHit hit;
    GameObject lastTile;


    // Add point variables
    bool addingPoint = false;
    public override void OnInspectorGUI()
    {
        enemyScript = (Enemy)target;
        DrawDefaultInspector();

        if(GUILayout.Button("Add patrol point") && !addingPoint)
        {
            addingPoint = true;
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI; // Just in case
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        if(addingPoint)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive); 

            GameObject tile = TileRaycaster.RayCastFloor();

            if (tile)
            {
                GUIUtility.hotControl = controlID;

                Handles.ConeCap(controlID, tile.transform.position, Quaternion.Euler(90, 0, 0), 1.0f);
                if(tile.GetComponent<FloorTile>().IsWalkable)
                {
                    lastTile = tile;

                    if (Event.current.type == EventType.MouseDown)
                    {
                        enemyScript.patrolSO.patrolPoint.Concat(new Vector3[] { tile.transform.position });
                        addingPoint = false;
                    }
                }               
            }
            else
            {
                lastTile = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            GUIUtility.hotControl = 0;
        }

        Vector3? previous = null;
       /* foreach(Vector3 point in enemyScript.patrolSO.patrolPoint)
        {
            if (previous != null) 
                Handles.DrawLine(previous.Value, point);

            previous = point;

            Handles.ConeCap(GUIUtility.hotControl, point, Quaternion.Euler(90,0,0), 1.0f);
        }*/


    }
}
