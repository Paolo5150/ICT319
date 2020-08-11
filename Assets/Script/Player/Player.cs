using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour, IShootable
{
    
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

    Health health;

    int raycastPlane;
    Rifle rifle;

    // Start is called before the first frame update
    public void Init()
    {
        raycastPlane = LayerMask.GetMask("RaycastPlane");
        rifle = GetComponentInChildren<Rifle>();
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
            RaycastHit[] hits = rifle.Shoot(fireRate, this.gameObject, damageGiven);
           

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
        health.Add(damage);
        if(health.IsDead())
        {
            //GameOver
        }
    }
}
