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
            rifle.Shoot(fireRate, this.gameObject, damageGiven);
            if (OnShotFired != null)
                OnShotFired();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
       
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
    }
}
