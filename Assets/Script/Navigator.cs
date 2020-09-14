using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigator : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent navAgent;
    [HideInInspector]
    public NavMeshPath path;

    public bool isMoving { get; private set; }
    public float walkSpeed = 0.1f;
    public float runSpeed = 0.5f;
    private Vector2 posVec2;
    private bool doneChecking = true;
    private System.Action onDestinationReachedListener;
    public float currentSpeed { get; private set; }
    public Vector2 GetPositionVec2()
    {
        return posVec2;
    }



    public void SetOnDestinationReachedListener(System.Action listener)
    {
        onDestinationReachedListener = listener;
    }

    // Start is called before the first frame update
    public void Init()
    {
        navAgent = GetComponent<NavMeshAgent>();
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseWalkSpeed()
    {
        currentSpeed = walkSpeed;

    }

    public void UseRunSpeed()
    {
        currentSpeed = runSpeed;

    }

    public void Stop()
    {
        StopAllCoroutines();
        isMoving = false;
        doneChecking = true;
        navAgent.enabled = false;
    }

    IEnumerator StartAgent(Vector3 end)
    {
        navAgent.enabled = true;
        navAgent.isStopped = true;

        yield return null;

    }

    IEnumerator CheckDistance()
    {
        yield return new WaitForSeconds(1.0f);
        while(navAgent.remainingDistance > 0.1)
        {
            yield return null;
        }

        Stop();
        if (onDestinationReachedListener != null)
            onDestinationReachedListener();

    }

    public void Go(Vector3 end)
    {
        doneChecking = true;
        StopAllCoroutines();
        isMoving = true;

        // StartCoroutine(StartAgent(end));
        navAgent.enabled = true;
        navAgent.speed = currentSpeed;
        navAgent.destination = end;
        navAgent.isStopped = false;
        StartCoroutine(CheckDistance());


    }

  
}
