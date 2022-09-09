using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI: MonoBehaviour
{
    [Header("Movement + Patrolling")]
    [Tooltip("Navmesh Agent")]
    public NavMeshAgent agent;
    [Tooltip("Patrol Points")]
    public Transform[] patrolPoints;
    [Tooltip("Patrol Point Index")]
    public int patrolPointIndex;
    [Tooltip("Ignore Patrol")]
    public bool ignorePatrol;
    [Tooltip("Patrol Point Wait Time")]
    public float patrolPointWait;

    Vector3 target;
    float waitTimer;
    bool movingOn;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        waitTimer = patrolPointWait;
        movingOn = false;

        if(!ignorePatrol)
        {
            GoToPatrolPoint();
        }
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

