using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Navigator))]
public class NavigatorInspector : Editor
{
    Navigator navigatorScript;
    void OnEnable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI; // Just in case
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        navigatorScript = (Navigator)target;

        if(navigatorScript.path != null)
        {

            foreach (Vector3 v in navigatorScript.path.corners)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(v, new Vector3(1, 1, 1));
            }
        }
    }
}
