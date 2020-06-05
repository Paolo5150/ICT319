using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
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

    public Navigator navigator;

   

    // Start is called before the first frame update
    public void Init()
    {
        navigator = GetComponent<Navigator>();

        navigator.SetOnReachedEndTileListener(() => {

            Camera.main.GetComponent<CameraRay>().Clear();
            GameManager.Instance.mapLoad = GameManager.Instance.mapLoad == "1" ? "2" : "1";
            GameManager.Instance.LoadMap();

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
   

   
}
