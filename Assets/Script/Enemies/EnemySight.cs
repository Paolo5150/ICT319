using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public float viewRange = 3.0f;
    public float FOV = 120.0f;

    public Ray rayToPlayer;
    RaycastHit hit;

    private bool looking;

    Action onPlayerSighted;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetOnPlayerSightedListener(Action listener)
    {
        onPlayerSighted = listener;
    }
    public void StartLookingForPlayer()
    {
        StopAllCoroutines();
        looking = true;
        StartCoroutine(LookForPlayer());
    }

    public void StopLookingForPlayer()
    {
        looking = false;
        StopAllCoroutines();
    }

    IEnumerator LookForPlayer()
    {
        while(looking)
        {
            if(IsPlayerInSight())
            {
                if (onPlayerSighted != null)
                    onPlayerSighted();
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
    public bool IsPlayerInSight()
    {
        bool inSight = false;
        Vector3 p = Player.Instance.transform.position;
        p.y = transform.position.y;

        Vector3 toPlayer = p - transform.position;
        if (Vector3.Magnitude(toPlayer) <= viewRange)
        {
            rayToPlayer.origin = transform.position;

            rayToPlayer.direction = toPlayer.normalized;
            float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
            
            if(angle < FOV / 2.0)
            {
                if (Physics.Raycast(rayToPlayer, out hit, viewRange))
                {
                    if (hit.transform.gameObject.name.Equals("Player"))
                    {
                        if (onPlayerSighted != null)
                            onPlayerSighted();
                    }
                }
            }
            
        }
        


        return inSight;
    }
}
