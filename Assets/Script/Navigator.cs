using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigator : MonoBehaviour
{
    public NavMeshAgent navAgent;

    private Vector2 posVec2;
    private bool doneChecking = false;
    private System.Action onEndTileListener;

    public Vector2 GetPositionVec2()
    {
        return posVec2;
    }

    public void SetOnReachedEndTileListener(System.Action listener)
    {
        onEndTileListener = listener;
    }

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(CheckNavAgent());
    }


    IEnumerator CheckNavAgent()
    {
        yield return new WaitForSeconds(1.0f);
        while(!doneChecking)
        {
            if (navAgent != null)
            {
                if (navAgent.remainingDistance < 0.1)
                    navAgent.isStopped = true;

                posVec2.x = transform.position.x;
                posVec2.y = transform.position.z;

                if(Vector2.Distance(posVec2, GameManager.Instance.GetCurrentMapEndTilePos()) < 0.1f)
                {
                    if (onEndTileListener != null)
                    {
                        doneChecking = true;
                        navAgent.enabled = false;
                        onEndTileListener();
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpeed(float speed)
    {
        navAgent.speed = speed;
    }

    IEnumerator StartAgent(Vector3 end)
    {
        navAgent.enabled = true;
        navAgent.SetDestination(end);

        while (!navAgent.hasPath)
            yield return null;

        navAgent.isStopped = false;

    }

    public void Go(Vector3 end)
    {
        doneChecking = true;
        StopAllCoroutines();
        StartCoroutine(StartAgent(end));
        doneChecking = false;

        StartCoroutine(CheckNavAgent());

    }

    public void Init()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }
}
