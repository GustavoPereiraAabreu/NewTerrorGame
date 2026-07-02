using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Spotted, Chase, Investigate, Search }
    [Header("Current State")] public State state = State.Patrol;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private JumpscareController jumpscareManager;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioSource chaseMusicAudio;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip walkStepsClip;
    [SerializeField] private AudioClip runStepsClip;

    [Header("Vision & Hearing")]
    [SerializeField] private float viewDistance = 12f;
    [SerializeField] private float viewAngle = 100f;
    [SerializeField] private float hearingRange = 10f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float stopDistance = 2.2f;

    [Header("Timers")]
    [SerializeField] private float reactionTime = 1.5f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float searchDuration = 8f;
    [SerializeField] private float startDelay = 0f;

    [Header("Footsteps Distance Volume")]
    [SerializeField] private float maxStepDistance = 20f;
    [SerializeField] private float minVolume = 0.05f;
    [SerializeField] private float maxVolume = 0.8f;

    [Header("Hiding Mechanic")]
    public bool isPlayerHidden = false;

    int patrolIndex;
    float stateTimer;
    bool heardNoise, aiActive, jumpscareTriggered;
    Vector3 targetPos;
    Vector3 searchSubPoint;
    bool firstSubPointSelected;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

        if (footstepAudio)
        {
            footstepAudio.loop = true;
            footstepAudio.clip = walkStepsClip;
            footstepAudio.volume = minVolume;
            footstepAudio.Play();
        }

        if (chaseMusicAudio)
        {
            chaseMusicAudio.loop = true;
            chaseMusicAudio.Stop();
        }

        MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (!aiActive) { HandleStartDelay(); return; }
        if (jumpscareTriggered) { StopAllEnemyAudio(); UpdateAnimation(0, false, false); return; }

        UpdateAudioSystems();
        if (state != State.Spotted && state != State.Chase) DetectPlayer();
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Patrol:
                ExecuteMovement(patrolSpeed, true);
                if (TargetReached(1.6f))
                {
                    if (agent.velocity.magnitude > 0.1f) stateTimer = 0f;
                    if (stateTimer >= waitTime) MoveToNextPatrolPoint();
                }
                break;

            case State.Spotted: ExecuteSpotted(); break;
            case State.Chase: ExecuteChase(); break;

            case State.Search:
                ExecuteMovement(patrolSpeed, true);
                agent.SetDestination(targetPos);
                UpdateAnimation(agent.velocity.magnitude, false, true);

                if (TargetReached(1.6f))
                {
                    firstSubPointSelected = false;
                    ChangeState(State.Investigate);
                }
                break;

            case State.Investigate:
                ExecuteInvestigateArea();
                break;
        }

        if (state != State.Spotted && state != State.Investigate && state != State.Search)
            UpdateAnimation(agent.velocity.magnitude, false, false);
    }

    void ChangeState(State newState)
    {
        state = newState;
        stateTimer = 0f;
    }

    void HandleStartDelay()
    {
        stateTimer += Time.deltaTime;
        if (HasValidAgent()) agent.isStopped = true;
        if (stateTimer >= startDelay) { aiActive = true; if (HasValidAgent()) agent.isStopped = false; stateTimer = 0f; }
    }

    void UpdateAudioSystems()
    {
        if (!player) return;

        if (footstepAudio)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float targetVolume = Mathf.Clamp01(1 - (distance / maxStepDistance));
            float currentVolume = Mathf.Lerp(minVolume, maxVolume, targetVolume);

            if (agent.velocity.magnitude <= 0.1f && state != State.Spotted)
            {
                currentVolume = 0f;
            }

            footstepAudio.volume = currentVolume;

            if (state == State.Spotted || state == State.Chase)
            {
                if (footstepAudio.clip != runStepsClip && runStepsClip != null)
                {
                    footstepAudio.clip = runStepsClip;
                    if (currentVolume > 0f && !footstepAudio.isPlaying) footstepAudio.Play();
                }
            }
            else
            {
                if (footstepAudio.clip != walkStepsClip && walkStepsClip != null)
                {
                    footstepAudio.clip = walkStepsClip;
                    if (currentVolume > 0f && !footstepAudio.isPlaying) footstepAudio.Play();
                }
            }

            if (currentVolume > 0f && !footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
        }

        if (state == State.Spotted || state == State.Chase)
        {
            if (chaseMusicAudio && !chaseMusicAudio.isPlaying)
            {
                chaseMusicAudio.Play();
            }
        }
        else
        {
            if (chaseMusicAudio && chaseMusicAudio.isPlaying)
            {
                chaseMusicAudio.Stop();
            }
        }
    }

    void StopAllEnemyAudio()
    {
        if (footstepAudio) footstepAudio.Stop();
        if (chaseMusicAudio) chaseMusicAudio.Stop();
    }

    void DetectPlayer()
    {
        if (isPlayerHidden) return;

        Vector3 eye = transform.position + Vector3.up * 1.6f;
        Vector3 dir = (player.position - eye).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= viewDistance && Vector3.Angle(transform.forward, dir) <= viewAngle / 2f)
        {
            if (Physics.Raycast(eye, dir, out RaycastHit hit, viewDistance, visionMask) && hit.collider.CompareTag("Player") && !Physics.Raycast(eye, dir, dist, obstacleMask))
            {
                targetPos = player.position;
                ChangeState(State.Spotted);
            }
        }
    }

    void ExecuteSpotted()
    {
        if (!HasValidAgent()) return;
        agent.isStopped = true; agent.velocity = Vector3.zero; agent.updateRotation = false;
        Vector3 lookDir = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up);
        if (lookDir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 15f);
        UpdateAnimation(0, true, false);
        if (stateTimer >= reactionTime) { agent.updateRotation = true; ChangeState(State.Chase); }
    }

    void ExecuteChase()
    {
        if (!HasValidAgent()) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= stopDistance && !jumpscareTriggered) { jumpscareTriggered = true; jumpscareManager.TriggerJumpscare(); return; }

        ExecuteMovement(chaseSpeed, true);
        agent.SetDestination(player.position);
        targetPos = player.position;

        if (isPlayerHidden || dist > viewDistance * 1.3f)
        {
            heardNoise = false;
            agent.ResetPath();
            ChangeState(State.Search);
        }
    }

    void ExecuteInvestigateArea()
    {
        if (!HasValidAgent()) return;

        ExecuteMovement(patrolSpeed, true);
        UpdateAnimation(agent.velocity.magnitude, false, true);

        if (!firstSubPointSelected || TargetReached(1.2f))
        {
            searchSubPoint = GetRandomPointInArea(targetPos, 5f);
            agent.SetDestination(searchSubPoint);
            firstSubPointSelected = true;
        }

        if (stateTimer >= searchDuration)
        {
            MoveToNextPatrolPoint();
        }
    }

    Vector3 GetRandomPointInArea(Vector3 center, float radius)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir += center;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, radius, 1))
        {
            return hit.position;
        }
        return center;
    }

    public void HearNoise(Vector3 pos)
    {
        if (isPlayerHidden || state == State.Spotted || state == State.Chase || Vector3.Distance(transform.position, pos) > hearingRange) return;

        heardNoise = true;
        targetPos = pos;
        if (HasValidAgent()) agent.ResetPath();
        ChangeState(State.Search);
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        if (state == State.Investigate) heardNoise = false;
        patrolIndex = Random.Range(0, patrolPoints.Length);
        ExecuteMovement(patrolSpeed, true);
        agent.SetDestination(patrolPoints[patrolIndex].position);
        ChangeState(State.Patrol);
    }

    void ExecuteMovement(float speed, bool updateRot)
    {
        if (!HasValidAgent()) return;
        agent.speed = speed; agent.updateRotation = updateRot; agent.isStopped = false;
    }

    bool TargetReached(float stopDist) => HasValidAgent() && !agent.pathPending && agent.remainingDistance < stopDist;
    bool HasValidAgent() => agent != null && agent.enabled && agent.isOnNavMesh;

    void UpdateAnimation(float speed, bool isSpotted, bool isSearching)
    {
        if (!animator) return;
        animator.SetBool("isWalking", speed > 0.1f && state != State.Chase);
        animator.SetBool("isRun", speed > 0.1f && state == State.Chase);
        animator.SetBool("isSpotted", isSpotted);
        animator.SetBool("isSearching", isSearching);
        animator.SetBool("isIdle", speed <= 0.1f && !isSpotted && !isSearching);
    }

    public void FreezeEnemy()
    {
        StopAllEnemyAudio();
        if (HasValidAgent()) { agent.isStopped = true; agent.ResetPath(); agent.enabled = false; }
        UpdateAnimation(0, false, false); enabled = false;
    }
}