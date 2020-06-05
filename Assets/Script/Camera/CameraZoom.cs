using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
    public static float origDistance;
    public static float currentDistance;
    bool first = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touch0, touch1;


        if (Input.touchCount >= 2)
        {

            if(first)
            {                
                touch0 = Input.GetTouch(0).position;
                touch1 = Input.GetTouch(1).position;
                origDistance = Vector2.Distance(touch0, touch1);
                currentDistance = origDistance;
                first = false;
            }
            else
            {
                touch0 = Input.GetTouch(0).position;
                touch1 = Input.GetTouch(1).position;
                currentDistance = Vector2.Distance(touch0, touch1);
            }

            Camera.main.orthographicSize -= (currentDistance - origDistance) / 20.0f;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5, 15);
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            origDistance = Vector2.Distance(touch0, touch1);

        }
        else
        {
            first = true;
        }

      /*  GameObject.Find("Canvas").transform.Find("Original").GetComponent<Text>().text = "Orig: " + origDistance;
        GameObject.Find("Canvas").transform.Find("Current").GetComponent<Text>().text = "Cur: " + currentDistance;
        GameObject.Find("Canvas").transform.Find("Difference").GetComponent<Text>().text = "Dif: " + Mathf.Abs(currentDistance - origDistance);
        GameObject.Find("Canvas").transform.Find("Size").GetComponent<Text>().text = "Size: " + Camera.main.orthographicSize;*/
    }
}
