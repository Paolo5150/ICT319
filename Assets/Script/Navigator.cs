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

    public void WalkSpeed()
    {
        currentSpeed = walkSpeed;

    }

    public void RunSpeed()
    {
        currentSpeed = runSpeed;

    }

    public void Stop()
    {
        StopAllCoroutines();
        currentSpeed = 0;
        doneChecking = true;
        navAgent.enabled = false;
    }

    IEnumerator Move(Vector3[] path)
    {
        float distance = 10000000;
        int current = 0;
        while (!doneChecking)
        {
            posVec2.x = transform.position.x;
            posVec2.y = transform.position.z;
            distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[current].x, path[current].z));
            if (distance < 0.1)
            {
                if (current < path.Length - 1)
                    current++;
                else
                {
                    isMoving = false;
                    if (onDestinationReachedListener != null)
                        onDestinationReachedListener();
                    doneChecking = true;
                    break;
                }
            }
            else
            {
                transform.LookAt(new Vector3(path[current].x, transform.position.y, path[current].z));
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[current].x, transform.position.y, path[current].z), Time.deltaTime * currentSpeed);
            }
            yield return null;
        }
    }

    IEnumerator StartAgent(Vector3 end)
    {
        navAgent.enabled = true;
        navAgent.isStopped = true;
        path = new NavMeshPath();
        navAgent.CalculatePath(end, path);
        doneChecking = false;

        isMoving = true;
        StartCoroutine(Move(path.corners));
        yield return null;

    }

    public void Go(Vector3 end)
    {
        doneChecking = true;
        StopAllCoroutines();
        StartCoroutine(StartAgent(end));


    }

  
}
