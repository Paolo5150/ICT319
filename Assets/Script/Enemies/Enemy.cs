using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{
    public delegate void SendAlarm(Vector3 playerSightPosition, GameObject triggeredBy);
    public static event SendAlarm OnAlarmSent;

    public float damageGiven;
    public float shootRate = 0.1f;

    public enum PersonalityEnum
    {
        BESERK,
        COWARD,
        SOLDIER
    }

    public PersonalityEnum enemyPersonality;

    [HideInInspector]
    public Navigator navigator;
    [HideInInspector]
    public EnemySight enemySight;
    [HideInInspector]
    public Rifle rifle;

    Sprite sosSprite;
 


    public Vector3 playerLastKnownPosition;
    public StateIcon stateIcon;
    Personality personality;
    public Health health;
    public void OnGetShot(GameObject from, float damage)
    {
        if(health != null)
        {
            health.Add(-damage);
            if(health.IsDead())
            {
                gameObject.SetActive(false);
                personality = null;
               // explosion.GetComponent<ExplosionEffect>().Trigger();
            }
        }

        if (personality != null)
            personality.OnGetShot(from);
    }

    void OnEnable()
    {
        Player.OnShotFired += OnPlayerShotFired;
        Player.OnPlayerDeath += OnPlayerDeath;
    }

    void OnDisable()
    {
        Player.OnShotFired -= OnPlayerShotFired;
        Player.OnPlayerDeath -= OnPlayerDeath;

        if (personality != null)
            personality.OnObjDisable();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void TriggerAlarm(Vector3 playerPos)
    {
        stateIcon.EnableTemporarily(sosSprite, 1.0f,false);
        if (OnAlarmSent != null)
            OnAlarmSent(playerPos, this.gameObject);
    }

    void OnPlayerDeath()
    {
        if (personality != null)
            personality.OnPlayerDeath();
    }

    public void Init()
    {
        if (!enabled) return;
        stateIcon = GetComponentInChildren<StateIcon>();
        stateIcon.Init();
        navigator = GetComponent<Navigator>();
        navigator.Init();

        health = new Health();
        rifle = GetComponentInChildren<Rifle>();
        enemySight = GetComponent<EnemySight>();

        enemySight.SetOnPlayerSightedListener(() => {
            if (personality != null)
                personality.OnPlayerSeen(Player.Instance.transform.position);
        });
        sosSprite = Resources.Load<Sprite>("StateIcons\\sos");
        ChoosePersonality();
    }

    private void Update()
    {
        if(personality != null)
            personality.Update();
    }

    void OnPlayerShotFired()
    {
        if(personality != null)
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
                GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
            case PersonalityEnum.SOLDIER:
                personality = new Soldier(this);
                GetComponent<MeshRenderer>().material.color = Color.gray;
                break;
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (personality != null)
            personality.OnTriggerEnter(c);
    }

}
