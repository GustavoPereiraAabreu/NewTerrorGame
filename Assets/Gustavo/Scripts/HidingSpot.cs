using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Transform hidingPosition;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private OutlineDetector outlineDetector;

    [Header("Player")]
    [SerializeField] private FirstPersonMovement playerMovement;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hideSound;

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionDistance = 3f;

    private Transform playerTransform;
    private CharacterController characterController;

    private bool isPlayerInside = false;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        characterController = playerTransform.GetComponent<CharacterController>();
    }

    void Update()
    {
        // Dentro do armário
        if (isPlayerInside)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                GetOut();
            }

            return;
        }


        // Fora do armário
        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out RaycastHit hit,
                            interactionDistance))
        {
            if (hit.transform == transform)
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    Hide();
                }
            }
        }
    }


    void Hide()
    {
        isPlayerInside = true;


        if (outlineDetector != null)
            outlineDetector.HideInteraction();


        if (enemyAI != null)
            enemyAI.isPlayerHidden = true;


        if (audioSource != null && hideSound != null)
            audioSource.PlayOneShot(hideSound);


        if (playerMovement != null)
            playerMovement.enabled = false;


        if (characterController != null)
            characterController.enabled = false;


        playerTransform.position = hidingPosition.position;
        playerTransform.rotation = hidingPosition.rotation;


        if (characterController != null)
            characterController.enabled = true;
    }


    void GetOut()
    {
        isPlayerInside = false;


        if (enemyAI != null)
            enemyAI.isPlayerHidden = false;


        if (characterController != null)
            characterController.enabled = false;


        if (exitPoint != null)
        {
            playerTransform.position = exitPoint.position;
            playerTransform.rotation = exitPoint.rotation;
        }


        if (characterController != null)
            characterController.enabled = true;


        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}