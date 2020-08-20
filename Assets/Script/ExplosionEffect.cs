using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplosionEffect : MonoBehaviour
{
    public bool available = true;
    NavMeshObstacle navMeshObstacle;
    ParticleSystem[] particlesSystems;
    Light myLight;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        particlesSystems = GetComponentsInChildren<ParticleSystem>();
        myLight = GetComponentInChildren<Light>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        myLight.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = false;
    }

    public void Drop(Vector3 position)
    {
        transform.position = position;
        navMeshObstacle.carvingTimeToStationary = 0.0f;
        navMeshObstacle.carving = false;
        Reset();
    }

    public void Carve(bool carve)
    {
        navMeshObstacle.carving = carve;
    }

    private void Reset()
    {
        myLight.enabled = false;

        GetComponent<BoxCollider>().enabled = true;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = true;
        available = false;
    }


    IEnumerator Detonate()
    {
        navMeshObstacle.carving = false;
        myLight.enabled = true;
        foreach (ParticleSystem p in particlesSystems)
            p.Play();

        GetComponent<BoxCollider>().enabled = false;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = false;

        yield return new WaitForSeconds(0.5f);
        myLight.enabled = false;
        available = true;

    }
    public void Trigger()
    {
        StartCoroutine(Detonate());
    }

    void OnTriggerEnter(Collider c)
    {

        if (c.tag.Equals("Enemy"))
        {
            Trigger();
            c.gameObject.GetComponent<IShootable>().OnGetShot(gameObject, 25.0f);


        }
 
    }
}
