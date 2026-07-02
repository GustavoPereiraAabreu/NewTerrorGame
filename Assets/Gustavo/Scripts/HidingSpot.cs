using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Transform hidingPosition;

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private Transform playerTransform;
    private MonoBehaviour playerMovementScript;
    private Collider playerCollider;

    private bool isPlayerInside = false;
    private Vector3 originalPlayerPosition;
    private bool canInteract = false;

    void Update()
    {
        if (canInteract && Input.GetKeyDown(interactionKey))
        {
            if (isPlayerInside)
                GetOut();
            else
                Hide();
        }
    }

    void Hide()
    {
        isPlayerInside = true;

        if (enemyAI != null)
        {
            enemyAI.isPlayerHidden = true;
        }

        originalPlayerPosition = playerTransform.position;

        if (playerMovementScript) playerMovementScript.enabled = false;

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        playerTransform.position = hidingPosition.position;
        playerTransform.rotation = hidingPosition.rotation;
    }

    void GetOut()
    {
        isPlayerInside = false;

        if (enemyAI != null)
        {
            enemyAI.isPlayerHidden = false;
        }

        playerTransform.position = originalPlayerPosition;

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc) cc.enabled = true;

        if (playerMovementScript) playerMovementScript.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            playerTransform = other.transform;
            playerMovementScript = other.GetComponent<MonoBehaviour>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}