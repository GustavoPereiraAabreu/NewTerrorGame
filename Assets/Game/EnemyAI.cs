using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Spotted, Chase, Search, Investigate }

    [Header("State")]
    public State state = State.Patrol;

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] JumpscareController jumpscareManager;

    [Header("Audio")]
    [SerializeField] AudioSource footstepAudio;
    [SerializeField] AudioSource chaseMusicAudio;
    [SerializeField] AudioSource voiceAudio;

    [SerializeField] AudioClip walkStepsClip;
    [SerializeField] AudioClip runStepsClip;

    [SerializeField] AudioClip[] calmLines;
    [SerializeField] AudioClip[] searchLines;
    [SerializeField] AudioClip[] investigateLines;
    [SerializeField] AudioClip[] chaseLines;

    [Header("Voice")]
    [SerializeField] float minTimeBetweenLines = 7;
    [SerializeField] float maxTimeBetweenLines = 15;

    [Header("Vision")]
    [SerializeField] float viewDistance = 12;
    [SerializeField] float viewAngle = 100;
    [SerializeField] float hearingRange = 10;
    [SerializeField] LayerMask visionMask;
    [SerializeField] LayerMask obstacleMask;

    [Header("Movement")]
    [SerializeField] float patrolSpeed = 2;
    [SerializeField] float chaseSpeed = 4;
    [SerializeField] float stopDistance = 2.2f;

    [Header("Timers")]
    [SerializeField] float reactionTime = 1.5f;
    [SerializeField] float waitTime = 2;
    [SerializeField] float searchDuration = 4;
    [SerializeField] float investigateDuration = 8;
    [SerializeField] float startDelay;

    [Header("Steps")]
    [SerializeField] float maxStepDistance = 20;
    [SerializeField] float minVolume = .05f;
    [SerializeField] float maxVolume = .8f;

    [Header("Hiding")]
    public bool isPlayerHidden;
    [SerializeField] float seeHideDistance = 6;

    int patrolIndex;
    float timer;
    float voiceTimer;
    float nextVoice;
    bool active;
    bool jumpscare;
    bool caughtHiding;
    bool firstPoint;

    Vector3 targetPos;
    Vector3 searchPoint;


    void Start()
    {
        agent ??= GetComponent<NavMeshAgent>();
        animator ??= GetComponent<Animator>();

        if (footstepAudio)
        {
            footstepAudio.loop = true;
            footstepAudio.clip = walkStepsClip;
            footstepAudio.Play();
        }

        if (chaseMusicAudio)
            chaseMusicAudio.Stop();

        ResetVoice();
        NextPatrol();
    }


    void Update()
    {
        if (!active)
        {
            timer += Time.deltaTime;

            if (timer >= startDelay)
            {
                active = true;
                timer = 0;
                agent.isStopped = false;
            }
            return;
        }


        if (jumpscare)
        {
            StopAudio();
            UpdateAnimation(0, false, false, false);
            return;
        }


        UpdateAudio();
        HandleVoice();

        if (state != State.Spotted && state != State.Chase)
            DetectPlayer();


        timer += Time.deltaTime;


        switch(state)
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

            case State.Search:
                Search();
                break;

            case State.Investigate:
                Investigate();
                break;
        }
    }


    void ChangeState(State s)
    {
        state = s;
        timer = 0;
    }


    void UpdateAudio()
    {
        if (!player || !footstepAudio)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        float volume = Mathf.Lerp(
            minVolume,
            maxVolume,
            1 - distance / maxStepDistance
        );

        bool moving = agent.velocity.magnitude > .1f;

        footstepAudio.volume = moving ? Mathf.Clamp(volume,0,maxVolume) : 0;


        bool running = state == State.Chase;

        AudioClip clip = running ? runStepsClip : walkStepsClip;

        if (footstepAudio.clip != clip)
        {
            footstepAudio.clip = clip;
            footstepAudio.Play();
        }


        if (chaseMusicAudio)
        {
            if (state == State.Chase && !chaseMusicAudio.isPlaying)
                chaseMusicAudio.Play();

            if (state != State.Chase && chaseMusicAudio.isPlaying)
                chaseMusicAudio.Stop();
        }
    }

        void HandleVoice()
    {
        if (!voiceAudio)
            return;

        voiceTimer += Time.deltaTime;

        if (voiceTimer < nextVoice || voiceAudio.isPlaying)
            return;

        AudioClip[] lines = calmLines;

        if (state == State.Chase || state == State.Spotted)
            lines = chaseLines;
        else if (state == State.Search)
            lines = searchLines;
        else if (state == State.Investigate)
            lines = investigateLines;

        PlayLine(lines);
        ResetVoice();
    }


    void PlayLine(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip)
        {
            voiceAudio.clip = clip;
            voiceAudio.Play();
        }
    }


    void ResetVoice()
    {
        voiceTimer = 0;
        nextVoice = Random.Range(minTimeBetweenLines, maxTimeBetweenLines);
    }


    void DetectPlayer()
    {
        if (isPlayerHidden)
            return;

        Vector3 eye = transform.position + Vector3.up * 1.6f;
        Vector3 dir = (player.position - eye).normalized;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > viewDistance)
            return;

        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2)
            return;


        if (Physics.Raycast(eye, dir, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.CompareTag("Player") &&
                !Physics.Raycast(eye, dir, distance, obstacleMask))
            {
                targetPos = player.position;

                PlayLine(chaseLines);

                ChangeState(State.Spotted);
            }
        }
    }


    void Patrol()
    {
        Move(patrolSpeed);

        if (Reached(1.6f))
        {
            if (agent.velocity.magnitude > 0.1f)
            {
                timer = 0f;
            }

            if (timer >= waitTime)
            {
                NextPatrol();
            }
        }
        else
        {
            timer = 0f;
        }

        UpdateAnimation(agent.velocity.magnitude, false, false, false);
    }


    void Spotted()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;


        Vector3 dir = Vector3.ProjectOnPlane(
            player.position - transform.position,
            Vector3.up
        );


        if(dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 15
            );
        }


        UpdateAnimation(0,true,false,false);


        if(timer >= reactionTime)
        {
            agent.updateRotation = true;
            ChangeState(State.Chase);
        }
    }


    void Chase()
    {
        float distance = Vector3.Distance(transform.position,player.position);


        if(distance <= stopDistance && !jumpscare)
        {
            jumpscare = true;
            jumpscareManager.TriggerJumpscare();
            return;
        }


        Move(chaseSpeed);
        agent.SetDestination(player.position);


        if(isPlayerHidden && distance <= seeHideDistance)
            caughtHiding = true;


        if((isPlayerHidden && !caughtHiding) ||
           distance > viewDistance * 1.3f)
        {
            agent.ResetPath();

            PlayLine(searchLines);

            ChangeState(State.Search);
        }


        UpdateAnimation(agent.velocity.magnitude,false,false,false);
    }


    void Search()
    {
        agent.isStopped = true;

        UpdateAnimation(0,false,true,false);


        if(timer >= searchDuration)
        {
            agent.isStopped = false;
            firstPoint = false;
            ChangeState(State.Investigate);
        }
    }


    void Investigate()
    {
        Move(patrolSpeed);

        if(!firstPoint || Reached(1.2f))
        {
            searchPoint = RandomPoint(targetPos,5);
            agent.SetDestination(searchPoint);
            firstPoint = true;
        }


        UpdateAnimation(agent.velocity.magnitude,false,false,true);


        if(timer >= investigateDuration)
            NextPatrol();
    }


    Vector3 RandomPoint(Vector3 center,float radius)
    {
        Vector3 pos = center + Random.insideUnitSphere * radius;

        if(NavMesh.SamplePosition(pos,out NavMeshHit hit,radius,1))
            return hit.position;

        return center;
    }


    public void HearNoise(Vector3 pos)
    {
        if(isPlayerHidden ||
           state == State.Spotted ||
           state == State.Chase ||
           Vector3.Distance(transform.position,pos) > hearingRange)
            return;


        targetPos = pos;
        agent.ResetPath();

        PlayLine(searchLines);

        ChangeState(State.Search);
    }


    void NextPatrol()
    {
        Transform[] activePoints = System.Array.FindAll(
            patrolPoints,
            point => point != null && point.gameObject.activeInHierarchy
        );

        if (activePoints.Length == 0)
            return;

        caughtHiding = false;

        Transform target = activePoints[Random.Range(0, activePoints.Length)];

        agent.SetDestination(target.position);

        ChangeState(State.Patrol);
    }


    void Move(float speed)
    {
        agent.speed = speed;
        agent.isStopped = false;
    }


    bool Reached(float distance)
    {
        return !agent.pathPending &&
               agent.remainingDistance < distance;
    }


    void UpdateAnimation(float speed,bool spotted,bool searching,bool investigating)
    {
        if(!animator)
            return;


        animator.SetBool("isWalking",
            speed > .1f &&
            state != State.Chase &&
            state != State.Investigate);


        animator.SetBool("isRun",
            speed > .1f &&
            state == State.Chase);


        animator.SetBool("isSpotted",spotted);
        animator.SetBool("isSearching",searching);
        animator.SetBool("isInvestig",investigating);

        animator.SetBool("isIdle",
            speed <= .1f &&
            !spotted &&
            !searching &&
            !investigating);
    }


    void StopAudio()
    {
        if(footstepAudio)
            footstepAudio.Stop();

        if(chaseMusicAudio)
            chaseMusicAudio.Stop();

        if(voiceAudio)
            voiceAudio.Stop();
    }


    public void FreezeEnemy()
    {
        StopAudio();

        if(agent)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        UpdateAnimation(0,false,false,false);

        enabled = false;
    }
}