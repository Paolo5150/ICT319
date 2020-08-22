using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    Slider playerHealthbar;
    [HideInInspector]
    public GameObject playerHUD;
    GameObject gameOverPanel;
    private static MainCanvas _instance;

    public static MainCanvas Instance
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

    public void Init()
    {
        playerHUD = transform.GetChild(0).gameObject;
        gameOverPanel = transform.GetChild(1).gameObject;

        playerHealthbar = playerHUD.GetComponentInChildren<Slider>();
        playerHealthbar.maxValue = Player.Instance.health.maxHealth;
        playerHealthbar.value = Player.Instance.health.GetHealth();

        gameOverPanel.SetActive(false);
    }

    public void UpdateHealth()
    {
        playerHealthbar.value = Player.Instance.health.GetHealth();

    }

    public void EnableGameOver(bool enable)
    {
        gameOverPanel.SetActive(enable);

    }




}
