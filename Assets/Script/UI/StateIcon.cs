using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateIcon : MonoBehaviour
{

    Quaternion originalRotation;
    Image image;

    Queue<Sprite> spritesQ;
    public void Init()
    {
        originalRotation = transform.rotation;
        image = GetComponentInChildren<Image>();
        image.enabled = false;
        spritesQ = new Queue<Sprite>();
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation * originalRotation;
        if(!image.enabled)
        {
            if(spritesQ.Count > 0)
            {
                image.sprite = spritesQ.Dequeue();
                StartCoroutine(Show(1.0f));
            }
        }
    }

    IEnumerator Show(float s)
    {

        image.enabled = true;
        yield return new WaitForSeconds(s);
        image.enabled = false;

    }

    public void EnableTemporarily(Sprite sprite, float seconds = 1.0f, bool willOverride = true)
    {
        if(willOverride)
        {
            StopAllCoroutines();
            image.sprite = sprite;
            StartCoroutine(Show(seconds));

        }
        else
        {
            spritesQ.Enqueue(sprite);

        }

    }
}
