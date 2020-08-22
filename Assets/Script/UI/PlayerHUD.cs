using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    Image bombImage;
    // Start is called before the first frame update
    void Start()
    {
        bombImage = transform.GetChild(1).GetComponent<Image>();
    }

    public void EnableBomb(bool enable)
    {
        bombImage.enabled = enable;
    }
}
