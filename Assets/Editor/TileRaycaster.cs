using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileRaycaster
{
    public static GameObject RayCastFloor()
    {
        Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        int floorTileLayer = LayerMask.GetMask("FloorTile");

        if (Physics.Raycast(r, out hit, 1000, floorTileLayer))
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    public static void AssignColorToMaterial(GameObject g, Color c)
    {
        var tempMaterial = new Material(g.GetComponent<Renderer>().sharedMaterial);
        tempMaterial.color = c;
        g.GetComponent<Renderer>().sharedMaterial = tempMaterial;
    }
}
