using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Spotted, Chase, Investigate, Search }
    [Header("Current State")] public State state = State.Patrol;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform[] patrolPoints;
    public AudioSource footstepAudio;
    public JumpscareController jumpscareManager;

    [Header("Vision & Hearing")]
    public float viewDistance = 12f;
    public float viewAngle = 100f;
    public float hearingRange = 10f;
    public LayerMask visionMask, obstacleMask;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float stopDistance = 2.2f;

    [Header("Timers")]
    public float reactionTime = 1.5f;
    public float waitTime = 2f;
    public float searchDuration = 7f;
    public float startDelay = 0f;

    [Header("Footsteps")]
    public float maxStepDistance = 20f;
    public float minVolume = 0.05f;
    public float maxVolume = 0.8f;

    int patrolIndex;
    float stateTimer;
    bool heardNoise, aiActive, jumpscareTriggered;
    Vector3 targetPos;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

        if (footstepAudio) { footstepAudio.loop = true; footstepAudio.volume = minVolume; footstepAudio.Play(); }
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (!aiActive) { HandleStartDelay(); return; }
        if (jumpscareTriggered) { UpdateAnimation(0, false); return; }

        UpdateFootsteps();
        if (state != State.Spotted && state != State.Chase) DetectPlayer();

        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Patrol:
                ExecuteMovement(patrolSpeed, true);
                if (TargetReached(1.6f))
                {
                    // CORREÇÃO: Reseta o timer no frame exato em que chega, iniciando os 2s de espera
                    if (agent.velocity.magnitude > 0.1f) stateTimer = 0f;
                    if (stateTimer >= waitTime) MoveToNextPatrolPoint();
                }
                break;

            case State.Spotted: ExecuteSpotted(); break;
            case State.Chase: ExecuteChase(); break;
            case State.Investigate: ExecuteMovement(patrolSpeed, true); agent.SetDestination(targetPos); if (TargetReached(1.6f)) ChangeState(State.Search); break;
            case State.Search: ExecuteMovement(patrolSpeed, true); if (stateTimer >= searchDuration) MoveToNextPatrolPoint(); break;
        }

        if (state != State.Spotted) UpdateAnimation(agent.velocity.magnitude, false);
    }

    void ChangeState(State newState) { state = newState; stateTimer = 0f; }

    void HandleStartDelay()
    {
        stateTimer += Time.deltaTime;
        if (HasValidAgent()) agent.isStopped = true;
        if (stateTimer >= startDelay) { aiActive = true; if (HasValidAgent()) agent.isStopped = false; stateTimer = 0f; }
    }

    void UpdateFootsteps()
    {
        if (!footstepAudio || !player) return;
        float t = Mathf.Clamp01(1 - (Vector3.Distance(transform.position, player.position) / maxStepDistance));
        footstepAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    void DetectPlayer()
    {
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

        UpdateAnimation(0, true);

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

        if (dist > viewDistance * 1.3f) { heardNoise = false; ChangeState(State.Investigate); }
    }

    public void HearNoise(Vector3 pos)
    {
        if (state == State.Spotted || state == State.Chase || Vector3.Distance(transform.position, pos) > hearingRange) return;
        heardNoise = true; targetPos = pos; ChangeState(State.Investigate);
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        if (state == State.Search) heardNoise = false;
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

    void UpdateAnimation(float speed, bool isSpotted)
    {
        if (!animator) return;
        animator.SetBool("isWalking", speed > 0.1f && state != State.Chase);
        animator.SetBool("isRun", speed > 0.1f && state == State.Chase);
        animator.SetBool("isSpotted", isSpotted);
        animator.SetBool("isIdle", speed <= 0.1f && !isSpotted);
    }

    public void FreezeEnemy()
    {
        if (HasValidAgent()) { agent.isStopped = true; agent.ResetPath(); agent.enabled = false; }
        UpdateAnimation(0, false); enabled = false;
    }
}