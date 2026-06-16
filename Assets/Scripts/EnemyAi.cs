using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 3.0f;          // Velocidade de movimento do inimigo

    [Header("Configurações de Detecção")]
    [SerializeField] private float detectionRange = 10.0f;  // Raio de visão do inimigo
    [SerializeField] private string playerTag = "Player";  // Tag que identifica o jogador

    [SerializeField] private Transform playerTransform;
    [SerializeField] private NavMeshAgent agent;

    void Start()
    {
        // Pega o componente NavMeshAgent anexado ao inimigo
        agent = GetComponent<NavMeshAgent>();

        // Procura pelo jogador
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calcula a distância até o jogador
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Se o jogador estiver dentro do raio de detecção, define o destino do agente
        if (distanceToPlayer <= detectionRange)
        {
            // O NavMesh calcula o caminho e move o inimigo sozinho
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            // Opcional: Faz o inimigo parar se o jogador fugir para longe
            agent.ResetPath();
        }
    }

    // Desenha o raio de detecção no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}