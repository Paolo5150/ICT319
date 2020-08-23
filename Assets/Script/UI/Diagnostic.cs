﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diagnostic : MonoBehaviour
{
    private static Diagnostic _instance;


    public static Diagnostic Instance
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

    [HideInInspector]
    public Text enemyTypeText;

    [HideInInspector]
    public Text damageGiveText;

    [HideInInspector]
    public Text stateText;

    [HideInInspector]
    public Text logsText;

    [HideInInspector]
    public GameObject enemyReference;

    int logLines;
    int logCounter;
    // Start is called before the first frame update
    void Start()
    {
        enemyTypeText = transform.GetChild(0).GetComponent<Text>();
        damageGiveText = transform.GetChild(1).GetComponent<Text>();
        stateText = transform.GetChild(2).GetComponent<Text>();
        logsText = transform.GetChild(3).GetComponent<Text>();
        SetEnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyReference != null)
        {
            stateText.text = "State: " + enemyReference.GetComponent<Enemy>().personality.stateMachine.GetCurrentState().stateName;
        }
    }

    public void setReference(GameObject e)
    {
        if (enemyReference != null && enemyReference != e)
        {
            enemyReference.GetComponent<Enemy>().usingDiagnostic = false;
        }


        enemyReference = e;
        enemyReference.GetComponent<Enemy>().usingDiagnostic = true;

        enemyTypeText.text = enemyReference.name;
        damageGiveText.text = "Damage: " + enemyReference.GetComponent<Enemy>().damageGiven;
        logsText.text = ""; // Clear logs
        logLines = 0;
        logCounter = 0;
    }

    string GetColor()
    {
        if (logCounter % 2 == 0)
            return "<color=white>";
        else
            return "<color=grey>";

    }
    public void AddLog(GameObject enemyObj, string log)
    {
        if(enemyObj == enemyReference)
        {
            logsText.text += logCounter + ". - " + GetColor() + log + "</color>\n";
            logLines++;
            logCounter++;
            if(logLines >=15)
            {
                logsText.text = "";
                logLines = 0;

            }
        }

    }
    public void SetEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }
}
