using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [Header("TARGET")]
    [SerializeField] private Transform playerTarget;

    [Header("NAVMESH & MOVEMENT")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int currentPointIndex = 0;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("DISTANCE SETTINGS")]
    [SerializeField] private float minSafeDistance = 7f;
    [SerializeField] private float patrolThreshold = 1.5f;

    [Header("CHASE SETTINGS")]
    [SerializeField] private float chaseSpeed = 5.5f;

    [Tooltip("Tempo em segundos que o monstro fica encarando o player antes de começar a correr.")]
    [SerializeField] private float reactionTime = 2f;

    [Tooltip("Tempo que ele continua correndo atrás após perder o player de vista.")]
    [SerializeField] private float loseTargetTimeout = 4f;

    [Header("--- VISION (EYES) ---")]
    [SerializeField] private float viewDistance = 15f;

    [Range(0f, 360f)] [SerializeField] private float viewAngle = 90f;
    [Tooltip("Camada (Layer) do cenário que bloqueia a visão do monstro (ex: Paredes).")]
    [SerializeField] private LayerMask obstacleMask;

    [Tooltip("Camada (Layer) em que o Player está configurado.")]
    [SerializeField] private LayerMask playerMask;

    // Estados da IA
    private enum EnemyState { Patrol, NoticingPlayer, Chasing }
    private EnemyState currentState = EnemyState.Patrol;

    private bool isChasingCoroutineRunning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
        {
            SetDestinationToPoint(currentPointIndex);
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // O monstro sempre tenta checar se está vendo o jogador com os "olhos"
        bool canSeePlayer = CheckFieldOfView();

        switch (currentState)
        {
            case EnemyState.Patrol:
                LookAtDirection(agent.velocity);
                ControlPatrolLoop();

                // Se viu o jogador na patrulha, para e começa a encarar
                if (canSeePlayer)
                {
                    StartCoroutine(NoticePlayerRoutine());
                }
                break;

            case EnemyState.NoticingPlayer:
                // Travado olhando fixamente para o jogador durante o tempo de reação
                LookAtTarget(playerTarget.position);
                agent.ResetPath(); // Para de andar
                break;

            case EnemyState.Chasing:
                LookAtTarget(playerTarget.position);

                if (canSeePlayer)
                {
                    // Atualiza a posição do jogador no NavMesh enquanto estiver vendo ele
                    agent.SetDestination(playerTarget.position);
                }
                else
                {
                    // Se perdeu de vista, inicia a contagem para desistir
                    if (!isChasingCoroutineRunning)
                    {
                        StartCoroutine(LostPlayerRoutine());
                    }
                }
                break;
        }
    }

    // Lógica dos "olhos" do monstro usando ângulo e raio de física
    private bool CheckFieldOfView()
    {
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;

        // Verifica se o jogador está dentro do ângulo de visão frontal
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            // Verifica se está dentro do limite de distância dos olhos
            if (distanceToPlayer <= viewDistance)
            {
                // Lança um raio (Raycast). Se não bater em nenhuma parede antes de chegar no player, ele viu!
                if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Rotina que faz o monstro encarar antes de correr
    private IEnumerator NoticePlayerRoutine()
    {
        currentState = EnemyState.NoticingPlayer;
        yield return new WaitForSeconds(reactionTime);

        // Após os segundos de encarada, se o player ainda estiver no alcance, ele corre
        if (Vector3.Distance(transform.position, playerTarget.position) <= viewDistance)
        {
            currentState = EnemyState.Chasing;
            agent.speed = chaseSpeed;
        }
        else
        {
            // Se o player sumiu muito rápido enquanto ele encarava, volta a patrulhar
            ResetToPatrol();
        }
    }

    // Rotina de perseguição quando perde o contato visual
    private IEnumerator LostPlayerRoutine()
    {
        isChasingCoroutineRunning = true;
        yield return new WaitForSeconds(loseTargetTimeout);

        // Se o tempo acabou e ele continuou sem ver o player, desiste
        if (!CheckFieldOfView())
        {
            ResetToPatrol();
        }
        isChasingCoroutineRunning = false;
    }

    private void ResetToPatrol()
    {
        currentState = EnemyState.Patrol;
        agent.speed = patrolSpeed;
        SetDestinationToPoint(currentPointIndex);
    }

    private void ControlPatrolLoop()
    {
        if (patrolPoints.Length == 0 || agent == null) return;

        if (!agent.pathPending && agent.remainingDistance <= patrolThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            SetDestinationToPoint(currentPointIndex);
        }
    }

    private void SetDestinationToPoint(int index)
    {
        if (patrolPoints[index] != null && agent.isOnNavMesh)
        {
            agent.SetDestination(patrolPoints[index].position);
        }
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 6f);
        }
    }

    private void LookAtDirection(Vector3 direction)
    {
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 4f);
        }
    }
}