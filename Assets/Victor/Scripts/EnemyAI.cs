using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Investigate, Search }
    public State state = State.Patrol;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
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

    [Header("Footsteps")]
    public float maxStepDistance = 20f;
    public float minVolume = 0.05f;
    public float maxVolume = 0.8f;

    [Header("Timers")]
    public float waitTime = 2f;
    public float searchDuration = 7f;
    public float startDelay = 0f;

    int patrolIndex;
    float waitTimer, searchTimer, startTimer;
    bool heardNoise, aiActive, jumpscareTriggered;

    Vector3 heardPos, lastSeenPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
        if (!aiActive || jumpscareTriggered) return;

        UpdateFootsteps();
        DetectPlayer();

        switch (state)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Investigate: Investigate(); break;
            case State.Search: Search(); break;
        }
    }

    void HandleStartDelay()
    {
        if (aiActive) return;

        startTimer += Time.deltaTime;
        agent.isStopped = true;

        if (startTimer >= startDelay)
        {
            aiActive = true;
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
                state = State.Chase;
            }
        }
    }

    public void HearNoise(Vector3 pos)
    {
        if (Vector3.Distance(transform.position, pos) <= hearingRange)
        {
            heardNoise = true;
            heardPos = pos;
            state = State.Investigate;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

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
        agent.speed = chaseSpeed;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= stopDistance)
        {
            if (!jumpscareTriggered)
            {
                jumpscareTriggered = true;

                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                agent.ResetPath();
                agent.enabled = false;

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
        agent.speed = patrolSpeed;
        searchTimer += Time.deltaTime;

        if (searchTimer >= searchDuration)
        {
            state = State.Patrol;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    public void FreezeEnemy()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;
        enabled = false;
    }
}