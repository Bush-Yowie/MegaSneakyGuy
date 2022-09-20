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
    [Tooltip("Surpised Delay")]
    public float surpriseDelay;
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
    float surpriseTimer;
    int investigationState;
    bool movingOn;
    bool ignorePatrolCheck;

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
        ignorePatrolCheck = ignorePatrol;
    }

    private void Update()
    {
        Detection();

        if (playerDetected && canSeePlayer)
        {
            //stop and shoot at player when within acceptable range, if not within exceptable range get closer, alert other ai after x time
        }
        else if (playerDetected && !canSeePlayer)
        {
            //go to player last seen location and if ai still cant see player have another countdown then go to hunting mode
        }
        else if (playerPartDetected)
        { 
            switch (investigationState)
            {
                //when entering investigation state, stop patrolling and look at player for x time
                case 0:
                    Debug.Log("INVESTIGATION STATE: STOPPED PATROL, LOOKING AT PLAYER");

                    ignorePatrol = true;

                    // looks at player while player in fov
                    if (canSeePlayer)
                    {
                        target = player.transform.position + new Vector3(0, headOffset, 0);
                        transform.LookAt(target);
                    }
                    else
                    {
                        investigationState = 1;
                    }
                    break;

                case 1:
                    Debug.Log("INVESTIGATION STATE: GOING TO PLAYER LAST KNOWN POSITION");

                    surpriseTimer = surpriseDelay;

                    //moves to last known player position then looks 180 degrees around
                    if (Vector3.Distance(transform.position, target) > 1)
                    {
                        MoveTo();
                    }
                    else
                    {
                        //slowly look right then left then if AI still hasnt seen anything go back to patrolling
                    }
                    break;
            }
            
        }
        //AI will move to set points in the order they are listed in patrol points list variable, staying at each point for a set time before moving to the next one
        if (!ignorePatrol)
        {
            Patrolling();
        }
        //AI will stop patrolling when ignorePatrol is set to true
        else if (ignorePatrolCheck != ignorePatrol && ignorePatrol) 
        {
            Debug.Log("AI was patrolling and should now stop patrolling");
            StopMoving();
            ignorePatrolCheck = ignorePatrol;
        }
    }

    //Send AI to target position
    private void MoveTo()
    {
        agent.SetDestination(target);
    }

    //Sends AI to it's current position
    private void StopMoving()
    {
        agent.SetDestination(transform.position);
    }

    private void Patrolling()
    {
        //AI will start patrolling when ignorePatrol is set to false
        if (ignorePatrolCheck != ignorePatrol && !ignorePatrol)
        {
            Debug.Log("AI was not patrolling and should now be patrolling");
            GoToPatrolPoint();
            ignorePatrolCheck = ignorePatrol;
        }
        //AI will start waiting at patrol point when reached
        if (Vector3.Distance(transform.position, target) < 1 && !movingOn)
        {
            Debug.Log("AI is at a patrol point and is not moving on");
            waitTimer -= Time.deltaTime;
        }
        //When AI has waited for full delay
        if (Vector3.Distance(transform.position, target) < 1 && waitTimer <= 0)
        {
            Debug.Log("AI is at a patrol point and the wait timer is at or below 0");
            movingOn = true;
            NextPatrolPoint();
            GoToPatrolPoint();
        }
        //When AI has left patrol point, reset timer and movingOn
        if (Vector3.Distance(transform.position, target) > 1 && movingOn)
        {
            Debug.Log("AI is not at a patrol point and is moving on");
            movingOn = false;
            waitTimer = patrolPointWait;
        }
    }

    //Sends AI to next patrol point
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
        if (detectionTimer <= detectDelay - visDelay * 8 && !playerDetected)
        {
            Debug.Log("Player has been in an AI's fov for long enough to make the AI investigate");
            playerPartDetected = true;
            investigationState = 0;
        }
        //if player in AI fov for full timer, go to combat state, ignore investigation state
        if (detectionTimer <= 0 && !playerDetected)
        {
            Debug.Log("Player has been in an AI's fov long enough to initiate combat");
            playerDetected = true;
            playerPartDetected = false;

            //reset timers when player is spotted
            detectionTimer = detectDelay;
            surpriseTimer = surpriseDelay;
        }
        //reset timer if player was in an AI's fov for long enough to trigger the timer but not long enough to trigger being spotted partially or fully (Imagine like coyote time for staying hidden) 
        if (detectionTimer < detectDelay && !playerDetected && !playerPartDetected && !canSeePlayer)
        {
            detectionTimer = detectDelay;
        }
    }
}

