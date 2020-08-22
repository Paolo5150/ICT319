using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplosionEffect : MonoBehaviour
{
    public bool available = true;
    public float bombReactivationTime = 10.0f;
    NavMeshObstacle navMeshObstacle;
    ParticleSystem[] particlesSystems;
    Light myLight;

    bool justDropped = false;
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
        Disable();
    }

    public void Drop(Vector3 position)
    {
        if(available)
        {
            transform.position = position;
            navMeshObstacle.enabled = true;

            navMeshObstacle.carvingTimeToStationary = 0.0f;
            navMeshObstacle.carving = false;
            PrepareOnGround();
            available = false;
            justDropped = true;
            MainCanvas.Instance.playerHUD.GetComponent<PlayerHUD>().EnableBomb(false);

        }
    }

    public void Carve(bool carve)
    {
        navMeshObstacle.carving = carve;
    }

    private void PrepareOnGround()
    {
        myLight.enabled = false;

        GetComponent<BoxCollider>().enabled = true;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = true;
        available = false;
        MainCanvas.Instance.playerHUD.GetComponent<PlayerHUD>().EnableBomb(false);
    }

    private void Disable()
    {
        navMeshObstacle.carving = false;
        myLight.enabled = false;


        GetComponent<BoxCollider>().enabled = false;
        navMeshObstacle.enabled = false;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = false;

    }


    IEnumerator Detonate()
    {
        navMeshObstacle.carving = false;
        myLight.enabled = true;
        foreach (ParticleSystem p in particlesSystems)
            p.Play();

        GetComponent<BoxCollider>().enabled = false;
        navMeshObstacle.enabled = false;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
            r.enabled = false;
        yield return new WaitForSeconds(1.0f);

        myLight.enabled = false;
        yield return new WaitForSeconds(bombReactivationTime);
        available = true;
        MainCanvas.Instance.playerHUD.GetComponent<PlayerHUD>().EnableBomb(true);

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
            c.GetComponent<Enemy>().OnGetBombed(25.0f);

        }
        else if (c.tag.Equals("Player")) 
        {
            if((!justDropped))
            {
                available = true;
                Disable();
                MainCanvas.Instance.playerHUD.GetComponent<PlayerHUD>().EnableBomb(true);

            }

        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag.Equals("Player"))
        {
            if(justDropped)
            {
                justDropped = false;
            }
        }
    }
}
