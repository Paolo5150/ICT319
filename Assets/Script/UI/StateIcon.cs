using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateIcon : MonoBehaviour
{

    Quaternion originalRotation;
    Image image;
    public void Init()
    {
        originalRotation = transform.rotation;
        image = GetComponentInChildren<Image>();
        image.enabled = false;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation * originalRotation;
    }

    IEnumerator Show(float s)
    {
        yield return new WaitForSeconds(0.1f);

        image.enabled = true;
        yield return new WaitForSeconds(s);
        image.enabled = false;

    }

    public void EnableTemporarily(Sprite sprite, float seconds = 1.0f)
    {
        StopAllCoroutines();
        image.sprite = sprite;
        StartCoroutine(Show(seconds));
    }
}
