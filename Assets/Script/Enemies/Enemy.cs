using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{

    public enum PersonalityEnum
    {
        BESERK,
        COWARD
    }

    public PersonalityEnum enemyPersonality;

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

    void OnEnable()
    {
        Player.OnShotFired += OnPlayerShotFired;
    }

    void OnDisable()
    {
        Player.OnShotFired -= OnPlayerShotFired;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        stateIcon = GetComponentInChildren<StateIcon>();
        stateIcon.Init();
        navigator = GetComponent<Navigator>();
        navigator.Init();

        health = new Health();
        rifle = GetComponentInChildren<Rifle>();
        enemySight = GetComponent<EnemySight>();
        enemySight.SetOnPlayerSightedListener(() => {
            playerLastKnownPosition = Player.Instance.transform.position;
        });

        ChoosePersonality();
    }

    private void Update()
    {
        if(personality != null)
            personality.Update();
    }

    void OnPlayerShotFired()
    {
        personality.OnPlayeShotFired(Player.Instance.transform.position);
    }

    void ChoosePersonality()
    {
        switch(enemyPersonality)
        {
            case PersonalityEnum.BESERK:
                personality = new Beserk(this);
                break;
            case PersonalityEnum.COWARD:
                personality = new Coward(this);
                break;
        }
    }

}
