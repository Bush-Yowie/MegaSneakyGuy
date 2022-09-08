using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI: MonoBehaviour
{
   // [Header("General")]
    //public Transform player;

    [Header("Movement + Patrolling")]
    [Tooltip("Navmesh Agent")]
    public NavMeshAgent agent;
    [Tooltip("Patrol Points")]
    public Transform[] patrolPoints;
    [Tooltip("Patrol Point Index")]
    public int patrolPointIndex;





    //public LayerMask whatIsGround, whatIsPlayer;

    //public Vector3 walkPoint;
    //bool patrolPointSet;
    //public float walkPointRange;

    //public float attackCooldown;
    //bool hasAttacked;

    //public float sightRange, attackRange;
    //public bool playerInSightRange, playerInAttackRange;

    Vector3 target;

    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        Patrolling();
        //NextPatrolPoint();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, target) < 1)
        {
            NextPatrolPoint();
            Patrolling();
        }
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        // playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //  if (!playerInSightRange && !playerInAttackRange)
        //  {
        //       Patrolling();
        //  }

        //  if(playerInSightRange && !playerInAttackRange)
        //   {
        //       CombatChasing();
        //   }

        //   if(playerInSightRange && playerInAttackRange)
        //   {
        //       CombatShooting();
        //   }

    }

    private void MoveTo()
    {
       // agent.SetDestination(target);
    }

    private void Patrolling()
    {
        target = patrolPoints[patrolPointIndex].position;
        agent.SetDestination(target);
        // if (!patrolPointSet)
        // {
        //SearchWalkPoint();
        // }

        //  if (patrolPointSet)
        //  {
        //      agent.SetDestination(walkPoint);
        // }

        // Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //checks to see if at patrol point
        //  if (distanceToWalkPoint.magnitude < 1f)
        //  {
        //      patrolPointSet = false;
        //   }
    }

    private void NextPatrolPoint()
    {
        patrolPointIndex++;

        if (patrolPointIndex == patrolPoints.Length)
        {
            patrolPointIndex = 0;
        }
    }

    // private void SearchWalkPoint()
    // {
    //     float randomZ = Random.Range(-walkPointRange, walkPointRange);
    //     float randomX = Random.Range(-walkPointRange, walkPointRange);

    //    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    //
    //    //checks if patrol point is out of bounds
    //     if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
    //     {
    //         patrolPointSet = true;
    //      }
    //   }

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

    private void ResetAttack()
    {
        //   hasAttacked = false;
    }

    private void Hunting()
    {

    }
}

