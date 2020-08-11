using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{
    public EnemyPatrolSO patrolSO;
    [HideInInspector]
    public Navigator navigator;
    [HideInInspector]
    public EnemySight enemySight;
    [HideInInspector]
    public Rifle rifle;

 
    public float damageGiven;
    public Vector3 playerLastKnownPosition;
    public StateIcon stateIcon;
    Personality personality;
    Health health;
    public void OnGetShot(GameObject from, float damage)
    {
        health.Add(-damage);
        if(health.IsDead())
        {
            gameObject.SetActive(false);
        }
    }

 
    // Start is called before the first frame update
    void Start()
    {
        stateIcon = GetComponentInChildren<StateIcon>();
        navigator = GetComponent<Navigator>();
        navigator.Init();
        health = new Health();
        rifle = GetComponentInChildren<Rifle>();
        enemySight = GetComponent<EnemySight>();
        personality = new Beserk(this);
        enemySight.SetOnPlayerSightedListener(() => {
            playerLastKnownPosition = Player.Instance.transform.position;
        });
    }


    private void Update()
    {
        personality.Update();
    }

}
