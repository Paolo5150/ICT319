using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{

    public float maxHealth = 100;

    public void Add(float h)
    {
        health += h;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    private float health = 100;
}
