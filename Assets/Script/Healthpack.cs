using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthpack : MonoBehaviour
{
    public static float HEALTH_GIVEN = 25.0f;
    public float rotationSpeed = 10.0f;

    private float reactivationTime = 25.0f;
    public bool isAvailable { get; private set; }
    private float timer;

    BoxCollider boxCollider;
    MeshRenderer mRenderer;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        mRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            boxCollider.enabled = true;
            mRenderer.enabled = true;
            isAvailable = true;
            timer = 0.0f;
        }

    }

    public void Reset()
    {
        isAvailable = false;
        boxCollider.enabled = false;
        mRenderer.enabled = false;
        timer = reactivationTime;
    }
}
