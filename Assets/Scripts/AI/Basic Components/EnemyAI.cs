using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
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
    [Tooltip("Player Partially Detected")]
    public bool playerPartDetected;

    Vector3 target;
    float waitTimer;
    float detectionTimer;
    bool movingOn;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        waitTimer = patrolPointWait;
        detectionTimer = detectDelay;
        movingOn = false;
        headPos = transform.position + new Vector3(0, headOffset, 0);

        //AI does not start patrolling when game is started without sending them to a point first, could maybe fix this by spawning them on a patrol point but this makes sure it always starts
        if (!ignorePatrol)
        {
            GoToPatrolPoint();
        }

        //lets the AI see
        StartCoroutine(VisionRoutine());
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Detection();

        if (playerDetected && canSeePlayer)
        {
            //combat code
        }
        else if (playerDetected && !canSeePlayer)
        {
            //have another countdown then go to hunting mode
        }
        else if (playerPartDetected)
        {
            //investigation code
        }
        //AI will move to set points in the order they are listed in patrol points list variable, staying at each point for a set time before moving to the next one
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
        //sets destination to its on position
        agent.SetDestination(transform.position);
    }

    private void Patrolling()
    {
        //when ai gets to patrol point, start waiting
        if (Vector3.Distance(transform.position, target) < 1 && !movingOn)
        {
            waitTimer -= Time.deltaTime;
        }
        //once ai has waited x time, go to next patrol point
        if (Vector3.Distance(transform.position, target) < 1 && waitTimer <= 0)
        {
            movingOn = true;
            NextPatrolPoint();
            GoToPatrolPoint();
        }
        //once ai is not at patrol point, reset wait timer
        if (Vector3.Distance(transform.position, target) > 1 && movingOn)
        {
            movingOn = false;
            waitTimer = patrolPointWait;
        }
    }

    //sends AI to next patrol point
    private void GoToPatrolPoint()
    {
        target = patrolPoints[patrolPointIndex].position;
        MoveTo();
    }

    //sets the next patrol point that the AI will move to, loops around to 0 when reaching the last point in the list
    private void NextPatrolPoint()
    {
        patrolPointIndex++;

        if (patrolPointIndex == patrolPoints.Length)
        {
            patrolPointIndex = 0;
        }
    }

    //runs infinitely with a 0.2 second delay between each repeat as to not crash the game/computer
    private IEnumerator VisionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(visDelay);

        while (true)
        {
            yield return wait;

            DetectionCheck();
        }
    }

    private void DetectionCheck()
    {
        //Checks if player is in vision range
        Collider[] rangeChecks = Physics.OverlapSphere(headPos, visRange, tarMask);

        //Only runs if player is in range
        if (rangeChecks.Length != 0)
        {
            //gets a reference to player position
            Transform visTarget = rangeChecks[0].transform;
            Vector3 directionToVisTarget = (visTarget.position - transform.position).normalized;

            //If player is in range and is in valid angle
            if (Vector3.Angle(transform.forward, directionToVisTarget) < visAngle / 2)
            {
                float distanceToVisTarget = Vector3.Distance(headPos, visTarget.position);

                //If player is in range, is in correct angle range and is not obstructed
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
        //sets can see player to false when player runs out of range after being spotted
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void Detection()
    {
        //while player in AI fov and not fully detected tick down timer
        if (canSeePlayer && !playerDetected)
        {
            detectionTimer -= Time.deltaTime;
            Debug.Log("Player is in an AI's fov, counting down");
        }
        //if player leaves AI fov but timer has ticked down a reasonable amount, AI will investigate
        if (detectionTimer <= detectDelay - visDelay * 2 && !canSeePlayer && !playerDetected)
        {
            Debug.Log("Player has been in an AI's fov for long enough to make the AI investigate");
            playerPartDetected = true;

            //reset timer when partially spotted
            detectionTimer = detectDelay;
        }
        //if player in AI fov for full timer, go to combat state, ignore investigation state
        if (detectionTimer <= 0 && !playerDetected)
        {
            Debug.Log("Player has been in an AI's fov long enough to initiate combat");
            playerDetected = true;
            playerPartDetected = false;

            //reset timer when player is spotted
            detectionTimer = detectDelay;
        }
        //reset timer if timer player was in an AI's fov for long enough to trigger the timer but not long enough to trigger being spotted partially or fully (Imagine like coyote time for staying hidden) 
        if (detectionTimer < detectDelay && !playerDetected && !playerPartDetected && !canSeePlayer)
        {
            detectionTimer = detectDelay;
        }
    }
}

