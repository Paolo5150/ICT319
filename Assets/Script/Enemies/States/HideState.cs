using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideState : EnemyState
{
    public bool doneHiding { get; private set; }
    public bool isHiding { get; private set; }

    bool canStartTimer;
    float safeRange = 10;
    float secondsToQuitState = 6.0f;
    float timer = 0;
    int wallLayer;

    public HideState(Personality e) : base(e)
    {
        stateImageSprite = Resources.Load<Sprite>("StateIcons\\hide");
        wallLayer = LayerMask.GetMask("Wall");

    }

    public void UpdatePos(Vector3 shotPos)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        var obstacles = GameManager.Instance.GetObstacles();

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(()=> {
            canStartTimer = true;
            isHiding = false;


        });

        timer = 0; // Reset timer
        doneHiding = false;
        canStartTimer = false;

        //Shuffle list
        int n = obstacles.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Vector3 value = obstacles[k];
            obstacles[k] = obstacles[n];
            obstacles[n] = value;
        }

        bool found = LookAndHIde(obstacles, false);
        if (!found)
            found = LookAndHIde(obstacles, true);

        //If a hiding spot wasn't found, the agent is probably cornered. Should never happen really 
        //Start the timer so it can get out of hiding state
        if(!found)
        {
            canStartTimer = true;
        }
    }

   
    public override void Update()
    {
        base.Update();
        if(!doneHiding && canStartTimer)
        {
            timer += Time.deltaTime;
            if (timer >= secondsToQuitState)
            {
                doneHiding = true;
                timer = 0;
            }
        }

        //Shoot at random in a state of panic!
        if (Random.Range(0, 100) < 5 && isHiding)
            personalityObj.enemyObj.rifle.Shoot(personalityObj.enemyObj.shootRate, personalityObj.enemyObj.gameObject, personalityObj.enemyObj.damageGiven);
 

    }
    public override void OnExit()
    {

        personalityObj.enemyObj.navigator.SetOnDestinationReachedListener(() => {

        });
    }

    private bool LookAndHIde(List<Vector3> obstacles, bool allowCrossPath)
    {
        foreach (Vector3 pos in obstacles)
        {
            Vector3 toPlayer = Player.Instance.transform.position - pos;
            Vector3 enemyToPlayer = Player.Instance.transform.position - personalityObj.enemyObj.transform.position;
            Vector3 enemyToOBstacle = pos - personalityObj.enemyObj.transform.position;
            if (toPlayer.magnitude > safeRange)
            {
                bool directionOk;
                if (allowCrossPath)
                    directionOk = true;
                else
                    directionOk = Vector3.Dot(enemyToPlayer.normalized, enemyToOBstacle.normalized) < 0;
                if (directionOk)
                {
                    //Look for hiding spot in that area
                    NavMesh.SamplePosition(pos, out NavMeshHit hit, 100, 1);

                    //Cast from player to potential hiding spot. If a wall is hit, the spot is hidden
                    Ray playerToSpot = new Ray(Player.Instance.transform.position, pos);
                    if (Physics.Raycast(playerToSpot, 500, wallLayer))
                    {
                        personalityObj.enemyObj.navigator.UseRunSpeed();
                        personalityObj.enemyObj.navigator.Go(hit.position);
                        isHiding = true;
                        return true;
                    }
                }
            }
        }

        return false;

    }
}
