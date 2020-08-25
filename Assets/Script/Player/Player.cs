using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour, IShootable
{
    public delegate void FireAction();
    public static event FireAction OnShotFired;

    public delegate void PlayerDeath();
    public static event PlayerDeath OnPlayerDeath;
    public ExplosionEffect bomb;

    private static Player _instance;


    public static Player Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public float moveSpeed = 3.0f;
    public float fireRate = 0.1f;
    public float damageGiven = 2;

    public Health health;

    int raycastPlane;
    Rifle rifle;
    float shootTimer = 0.0f;
    // Start is called before the first frame update
    public void Init()
    {
        raycastPlane = LayerMask.GetMask("RaycastPlane");
        rifle = GetComponentInChildren<Rifle>();
        rifle.lineRenderer.material.color = Color.red;
        health = new Health();
    }

    void Move()
    {
        RaycastHit mouseHit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out mouseHit, 200, raycastPlane))
        {
            transform.LookAt(new Vector3(mouseHit.point.x, transform.position.y, mouseHit.point.z));
        }

        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.position -= Vector3.right * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }

    void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootTimer >= fireRate)
            {
                rifle.Shoot(fireRate, this.gameObject, damageGiven);
                shootTimer = 0.0f;
                if (OnShotFired != null)
                    OnShotFired();
            }
            else
                shootTimer += Time.deltaTime;
  
        }
        else if(Input.GetMouseButton(1))
        {
            bomb.Drop(transform.position);

        }

        MainCanvas.Instance.playerHUDScript.SetAmmoText("" + rifle.Ammo);

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        Debug.Log("Ammo: " + rifle.Ammo);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale != 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
       
    }

    public void OnGetShot(GameObject from, float damage)
    {
        health.Add(-damage);
        MainCanvas.Instance.UpdateHealth();
        if(health.IsDead())
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            //GameOver
            if (OnPlayerDeath != null)
                OnPlayerDeath();

            MainCanvas.Instance.EnableGameOver(true);
        }
    }

    void OnTriggerEnter(Collider c)
    {

        if (c.tag.Equals("Healthpack"))
        {
            health.Add(Healthpack.HEALTH_GIVEN);
            MainCanvas.Instance.UpdateHealth();
            c.gameObject.GetComponent<Healthpack>().Reset();
        }
        if (c.tag.Equals("AmmoBox"))
        {
            rifle.AddAmmo(AmmoBox.AMMO_GIVEN);
            MainCanvas.Instance.playerHUDScript.SetAmmoText("" + rifle.Ammo);
            c.gameObject.GetComponent<AmmoBox>().Reset();
        }
    }
}
