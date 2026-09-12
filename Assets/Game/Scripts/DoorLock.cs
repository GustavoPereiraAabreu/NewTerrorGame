using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DoorLock : MonoBehaviour, Iinteractable
{
    [Header("Arraste a Porta Principal Aqui")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject interactionText;

    [Header("Objetos da Porta")]
    [SerializeField] private NavMeshObstacle navObstacle;

    [Header("Patrol Points Para Ativar")]
    [SerializeField] private GameObject[] patrolPoints;

    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    // Método exatamente igual a interface exige (sem parâmetros)
    public void Interact()
    {
        // Acha o script de coleta do player automaticamente na cena
        PlayerCollect player = FindAnyObjectByType<PlayerCollect>();

        if (player == null) return;

        // Pega o item que está na mão do player
        Transform itemNaMao = player._objects; // Mude para o nome da sua variável se for diferente

        // Se a mão estiver vazia, não faz nada
        if (itemNaMao == null)
        {
            Debug.Log("Preciso de uma chave para abrir o cadeado.");
            return;
        }

        // --- TEM ITEM NA MÃO: ABRE A PORTA E SOME COM O CADEADO ---
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (interactionText != null)
            interactionText.SetActive(false);

        foreach (GameObject point in patrolPoints)
        {
            if (point != null)
                point.SetActive(true);
        }

        // Destrói a chave que estava na mão
        Destroy(itemNaMao.gameObject);

        // Remove o obstáculo do NavMesh com atraso
        StartCoroutine(RemoverObstaculo());

        // Destrói o próprio cadeado
        Destroy(gameObject);
    }

    private IEnumerator RemoverObstaculo()
    {
        yield return new WaitForSeconds(1f);

        if (navObstacle != null)
            navObstacle.enabled = false;
    }

    public void ShowOutline()
    {
        if (_outline != null)
            _outline.enabled = true;
    }

    public void HideOutline()
    {
        if (_outline != null)
            _outline.enabled = false;
    }
}