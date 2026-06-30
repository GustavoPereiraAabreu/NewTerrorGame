using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Spotted, Chase, Investigate, Search }
    public State state = State.Patrol;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform[] patrolPoints;
    public AudioSource footstepAudio;
    public JumpscareController jumpscareManager;

    [Header("Vision")]
    public float viewDistance = 12f;
    public float viewAngle = 100f;
    public LayerMask visionMask;
    public LayerMask obstacleMask;

    [Header("Hearing")]
    public float hearingRange = 10f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Attack")]
    public float stopDistance = 2.2f;

    [Header("Reaction System")]
    public float reactionTime = 1.5f;

    [Header("Footsteps")]
    public float maxStepDistance = 20f;
    public float minVolume = 0.05f;
    public float maxVolume = 0.8f;

    [Header("Timers")]
    public float waitTime = 2f;
    public float searchDuration = 7f;
    public float startDelay = 0f;

    int patrolIndex;
    float waitTimer, searchTimer, startTimer, reactionTimer;
    bool heardNoise, aiActive, jumpscareTriggered;

    Vector3 heardPos, lastSeenPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (footstepAudio)
        {
            footstepAudio.loop = true;
            footstepAudio.volume = minVolume;
            footstepAudio.Play();
        }
    }

    void Update()
    {
        HandleStartDelay();

        if (!aiActive || jumpscareTriggered)
        {
            UpdateAnimation(false, false, false);
            return;
        }

        UpdateFootsteps();

      
        if (state != State.Spotted && state != State.Chase)
        {
            DetectPlayer();
        }

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Spotted:
                Spotted();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Investigate:
                Investigate();
                break;

            case State.Search:
                Search();
                break;
        }

        CheckMovementForAnimation();
    }

    void HandleStartDelay()
    {
        if (aiActive) return;

        startTimer += Time.deltaTime;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;

        if (startTimer >= startDelay)
        {
            aiActive = true;

            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.isStopped = false;
        }
    }

    void UpdateFootsteps()
    {
        if (!footstepAudio || !player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        float t = Mathf.Clamp01(1 - (dist / maxStepDistance));
        footstepAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    void DetectPlayer()
    {
        Vector3 eye = transform.position + Vector3.up * 1.6f;
        Vector3 dir = (player.position - eye).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > viewDistance) return;
        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2f) return;

        if (Physics.Raycast(eye, dir, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.CompareTag("Player") &&
                !Physics.Raycast(eye, dir, dist, obstacleMask))
            {
                lastSeenPos = player.position;

              
                reactionTimer = 0f;
                state = State.Spotted;
            }
        }
    }


    void Spotted()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

       
        agent.updateRotation = false;

       
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
           
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 15f);
        }

        reactionTimer += Time.deltaTime;

    
        if (reactionTimer >= reactionTime)
        {
            agent.updateRotation = true; 
            agent.isStopped = false;
            state = State.Chase;
        }
    }

    public void HearNoise(Vector3 pos)
    {
        if (state == State.Spotted || state == State.Chase) return;

        if (Vector3.Distance(transform.position, pos) <= hearingRange)
        {
            heardNoise = true;
            heardPos = pos;
            state = State.Investigate;
        }
    }

    void Patrol()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.updateRotation = true;
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0)
            return;

        if (heardNoise)
        {
            state = State.Investigate;
            return;
        }

        if (agent.remainingDistance < 0.3f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waitTimer = 0;
                patrolIndex = Random.Range(0, patrolPoints.Length);
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
        }
    }

    void Chase()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.updateRotation = true;
        agent.speed = chaseSpeed;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= stopDistance)
        {
            if (!jumpscareTriggered)
            {
                jumpscareTriggered = true;
                jumpscareManager.TriggerJumpscare();
            }
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);

        lastSeenPos = player.position;

        if (dist > viewDistance * 1.3f)
            state = State.Investigate;
    }

    void Investigate()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.updateRotation = true;
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        Vector3 target = heardNoise ? heardPos : lastSeenPos;
        agent.SetDestination(target);

        if (agent.remainingDistance < 0.4f)
        {
            heardNoise = false;
            searchTimer = 0;
            state = State.Search;
        }
    }

    void Search()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.updateRotation = true;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        searchTimer += Time.deltaTime;

        if (searchTimer >= searchDuration)
        {
            state = State.Patrol;

            if (patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void CheckMovementForAnimation()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            bool isMoving = agent.velocity.sqrMagnitude > 0.01f && !agent.isStopped;
            bool isRunning = isMoving && (state == State.Chase);
            bool isWalking = isMoving && !isRunning;
            bool isSpotted = (state == State.Spotted);

            UpdateAnimation(isWalking, isRunning, isSpotted);
        }
        else
        {
            UpdateAnimation(false, false, (state == State.Spotted));
        }
    }

    void UpdateAnimation(bool isWalking, bool isRunning, bool isSpotted)
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRun", isRunning);
            animator.SetBool("isSpotted", isSpotted);
            animator.SetBool("isIdle", !isWalking && !isRunning && !isSpotted);
        }
    }

    public void FreezeEnemy()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        UpdateAnimation(false, false, false);
        enabled = false;
    }
}