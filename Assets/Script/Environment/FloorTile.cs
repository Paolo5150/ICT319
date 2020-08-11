using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    private bool isWalkable = true;
    public bool IsWalkable
    {
        get
        {
            return isWalkable;
        }
        set
        {
            if (value)
            {
                transform.Find("Wall").gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.Find("Wall").gameObject.GetComponent<BoxCollider>().enabled = false;


            }
            else
            {
                transform.Find("Wall").gameObject.GetComponent<MeshRenderer>().enabled = true;
                transform.Find("Wall").gameObject.GetComponent<BoxCollider>().enabled = true;
                transform.Find("Wall").gameObject.GetComponent<BoxCollider>().isTrigger = false;
            }
                


            isWalkable = value;

        }
    }

    GameObject wallCube;
    // Start is called before the first frame update
    void Start()
    {
        wallCube = transform.Find("Wall").gameObject;
        if (IsWalkable)
            wallCube.GetComponent<MeshRenderer>().enabled = false;
        else
            wallCube.GetComponent<MeshRenderer>().enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

        
}
