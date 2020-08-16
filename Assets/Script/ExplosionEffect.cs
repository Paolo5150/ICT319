using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    ParticleSystem[] particlesSystems;
    Light myLight;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        particlesSystems = GetComponentsInChildren<ParticleSystem>();
        myLight = GetComponentInChildren<Light>();
        myLight.enabled = false;
    }

    public void Trigger()
    {
        myLight.enabled = true;
        foreach (ParticleSystem p in particlesSystems)
            p.Play();
    }
}
