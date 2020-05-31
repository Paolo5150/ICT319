using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public bool isWalkable = true;

    GameObject wallCube;
    // Start is called before the first frame update
    void Start()
    {
        wallCube = transform.Find("Wall").gameObject;
        if (isWalkable)
            wallCube.GetComponent<MeshRenderer>().enabled = false;
        else
            wallCube.GetComponent<MeshRenderer>().enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
