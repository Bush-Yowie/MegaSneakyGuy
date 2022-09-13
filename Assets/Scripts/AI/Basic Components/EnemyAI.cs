using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI: MonoBehaviour
{
    [Header("Movement + Patrolling")]
    [Tooltip("Patrol Point Wait Time")]
    public float patrolPointWait;
    [Tooltip("Patrol Point Index")]
    public int patrolPointIndex;
    [Tooltip("Navmesh Agent")]
    public NavMeshAgent agent;
    [Tooltip("Patrol Points")]
    public Transform[] patrolPoints;
    [Tooltip("Ignore Patrol")]
    public bool ignorePatrol;


    [Header("Vision + Detection")]
    [Tooltip("FOV Angle")]
    [Range(0, 360)]
    public float visAngle;
    [Tooltip("FOV Range")]
    public float visRange;
    [Tooltip("Vision Routine Repeat Delay")]
    public float visDelay;
    [Tooltip("Time To Detect Player")]
    public float detectDelay;
    [Tooltip("Head Height Offset")]
    public float headOffset;
    [Tooltip("Player Reference")]
    public GameObject player;
    [Tooltip("Obstruction Mask")]
    public LayerMask obMask;
    [Tooltip("Target Mask")]
    public LayerMask tarMask;
    [Tooltip("AI Head Position")]
    public Vector3 headPos;
    [Tooltip("Player In AI Vision")]
    public bool canSeePlayer;
    [Tooltip("Player Detected")]
    public bool playerDetected;

    Vector3 target;
    float waitTimer;
    bool movingOn;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        waitTimer = patrolPointWait;
        movingOn = false;
        headPos = transform.position + new Vector3 (0, headOffset, 0);

        if(!ignorePatrol)
        {
            GoToPatrolPoint();
        }

        StartCoroutine(VisionRoutine());
    }

    private void Update()
    {
        if (!ignorePatrol)
        {
            Patrolling();
        }

    }

    private void MoveTo()
    {
        agent.SetDestination(target);
    }

    private void StopMoving()
    {
        agent.SetDestination(transform.position);
    }

    private void Patrolling()
    {
        //once ai has waited x time, go to next patrol point
        if (Vector3.Distance(transform.position, target) < 1 && waitTimer <= 0)
        {
            //Debug.Log("AI wait count down has reached 0 or below, moving to next patrol point");
            movingOn = true;
            NextPatrolPoint();
            GoToPatrolPoint();
        }
        //once ai is not at patrol point, reset wait timer
        if (Vector3.Distance(transform.position, target) > 1 && movingOn)
        {
            //Debug.Log("AI has left previous patrol point, reseting timer");
            movingOn = false;
            waitTimer = patrolPointWait;
        }
        //when ai gets to patrol point, start waiting
        if (Vector3.Distance(transform.position, target) < 1 && !movingOn)
        {
            //Debug.Log("AI has reached patrol point, starting count down");
            waitTimer -= Time.deltaTime;
            //Debug.Log("Wait timer is currently at: " + waitTimer);
        }
    }

    private void GoToPatrolPoint()
    {
        target = patrolPoints[patrolPointIndex].position;
        MoveTo();
    }

    private void NextPatrolPoint()
    {
        patrolPointIndex++;

        if (patrolPointIndex == patrolPoints.Length)
        {
            patrolPointIndex = 0;
        }
    }

    private IEnumerator VisionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(visDelay);

        while(true)
        {
            yield return wait;

            DetectionCheck();
        }
    }

    private void DetectionCheck()
    {
        //Checks if player is in vision range
        Collider[] rangeChecks = Physics.OverlapSphere(headPos, visRange, tarMask);

        //Only runs if valid object is in vision range (In this case its the player)
        if (rangeChecks.Length != 0)
        {
            Transform visTarget = rangeChecks[0].transform;
            Vector3 directionToVisTarget = (visTarget.position - transform.position).normalized;

            //If player is in range and is in valid angle
            if (Vector3.Angle(transform.forward, directionToVisTarget) < visAngle / 2)
            {
                float distanceToVisTarget = Vector3.Distance(headPos, visTarget.position);

                if (!Physics.Raycast(headPos, directionToVisTarget, distanceToVisTarget, obMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void CombatChasing()
    {
    //    agent.SetDestination(player.position);
    }

    private void CombatShooting()
    {
     //   agent.SetDestination(transform.position);

     //   transform.LookAt(player);

        //  if (!hasAttacked)
        {
            //attack code goes here

            //        hasAttacked = true;
            //       Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    
}

