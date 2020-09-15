using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplosionEffect : MonoBehaviour, IShootable
{
    public bool available = true;
    public float bombReactivationTime = 10.0f;
    public float damageRadius = 5.0f;
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
        int wallLayer = LayerMask.GetMask("Wall");
        if (c.tag.Equals("Enemy"))
        {
            Trigger();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject e in enemies)
            {
                //Check if within radius
                Vector3 toEnemy = e.transform.position - transform.position;
                float dist = toEnemy.magnitude;

                if (dist <= damageRadius)
                {
                    //Check that enemy is not protected by wall
                    Ray r = new Ray(transform.position, toEnemy.normalized);
                    RaycastHit[] hit;
                    hit = Physics.RaycastAll(r, damageRadius);
                    if(hit.Length != 0)
                    {
                        foreach(RaycastHit enemyHit in hit)
                        {
                            if (enemyHit.collider.gameObject == e)
                            {
                                e.GetComponent<Enemy>().OnGetBombed(20);
                                break;
                            }
                        }
                    }
                 
                }
            }

            //Also check if player is in range
            Vector3 toPlayer = new Vector3(Player.Instance.transform.position.x, transform.position.y, Player.Instance.transform.position.z) - transform.position;

            if(toPlayer.magnitude < 0.0001) // Player dropped the bomb right on enemy
            {
                Player.Instance.GetComponent<Player>().OnGetBombed(20);
            }
            else if (toPlayer.magnitude <= damageRadius)
            {
                //Check that enemy is not protected by wall
                Ray r = new Ray(transform.position, toPlayer.normalized);
                RaycastHit[] hit;
                hit = Physics.RaycastAll(r, damageRadius);
                if (hit.Length != 0)
                {
                    foreach (RaycastHit pHit in hit)
                    {
                        if (pHit.collider.gameObject == Player.Instance.gameObject)
                        {
                            Player.Instance.GetComponent<Player>().OnGetBombed(20);
                            break;
                        }
                    }
                }

            }

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

    public void OnGetShot(GameObject from, float damage)
    {
        if(isActiveAndEnabled)
            Trigger();
    }

    public void OnGetBombed(float damage)
    {
        throw new System.NotImplementedException();
    }
}
