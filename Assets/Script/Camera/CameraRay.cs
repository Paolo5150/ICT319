﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    public float wallAlpha;
    public bool enableTileSelection;
    private Ray ray;
    private RaycastHit[] wallHit;
    private int wallLayer;
    private int floorTileLayer;
    private float pressTime = 0.0f;

    Color origColor;
    List<GameObject> lastHit;
    GameObject lastTile;
    GameObject selectedTile;

    MSCameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(transform.position, Player.Instance.transform.position);
        wallLayer = LayerMask.GetMask("Wall");
        floorTileLayer = LayerMask.GetMask("FloorTile");
        lastHit = new List<GameObject>();
        cameraController = GetComponent<MSCameraController>();
        cameraController.cameras[0].rotationType = MSACC_CameraType.TipoRotac.Orbital;

    }

    public void Clear()
    {
        lastHit.Clear();
        lastTile = null;
        selectedTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        ray.origin = transform.position;
        ray.direction = Player.Instance.transform.position - Vector3.up / 2.0f - ray.origin;

        wallHit = Physics.RaycastAll(ray, 1000.0f, wallLayer);
        if(wallHit.Length > 0)
        {
            foreach (GameObject hit in lastHit)
                hit.GetComponent<MeshRenderer>().material.color = new Color(origColor.r, origColor.g, origColor.b, 1.0f);

            foreach (RaycastHit hit in wallHit)
            {
                origColor = hit.transform.gameObject.GetComponent<MeshRenderer>().material.color;
                hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(origColor.r, origColor.g, origColor.b, wallAlpha);
                
                if (!lastHit.Contains(hit.transform.gameObject))
                    lastHit.Add(hit.transform.gameObject);

            }
            
        }
        else
        {
            foreach(GameObject hit in lastHit)
                hit.GetComponent<MeshRenderer>().material.color = new Color(origColor.r, origColor.g, origColor.b, 1.0f);

            lastHit.Clear();
        }

        if(true)
        {
            if (Input.GetMouseButton(0) && enableTileSelection)
            {
                RaycastHit mouseHit;
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(r, out mouseHit, 1000.0f, floorTileLayer))
                {
                    if (lastTile != null && lastTile != mouseHit.transform.gameObject)
                        lastTile.GetComponent<MeshRenderer>().material.color = Color.white;

                    FloorTile ft = mouseHit.transform.gameObject.GetComponent<FloorTile>();

                    if (ft.IsWalkable && pressTime == 0.0f)
                    {
                        enableTileSelection = true;
                        cameraController.enabled = false;

                    }

                    if (ft.IsWalkable && enableTileSelection)
                    {

                        pressTime += Time.deltaTime;

                        mouseHit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 1.0f);
                        selectedTile = mouseHit.transform.gameObject;
                        lastTile = mouseHit.transform.gameObject;
                    }
                }
                else
                    enableTileSelection = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (selectedTile != null && enableTileSelection)
                {
                    float speed = 2.0f;
                    if (pressTime > 1.0f)
                    {
                        speed = 4.0f;
                    }
                    Player.Instance.GetComponent<Navigator>().SetSpeed(speed);
                    Player.Instance.GetComponent<Navigator>().Go(selectedTile.transform.position);
                }
                cameraController.enabled = true;

                enableTileSelection = true;
                pressTime = 0.0f;
            }
        }       

    }
}