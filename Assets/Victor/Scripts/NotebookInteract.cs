using UnityEngine;

public class NotebookInteract : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject painelNotebook;
    public GameObject interactionText;

    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    public float distanciaInteracao = 3f;

    private bool aberto = false;

    void Start()
    {
        painelNotebook.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (aberto)
            return;

        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out RaycastHit hit,
                            distanciaInteracao))
        {
            if (hit.transform == transform && Input.GetKeyDown(KeyCode.E))
            {
                AbrirNotebook();
            }
        }
    }

    public void AbrirNotebook()
    {
        aberto = true;

        painelNotebook.SetActive(true);

        if (interactionText != null)
            interactionText.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.LockLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FecharNotebook()
    {
        aberto = false;

        painelNotebook.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.UnlockLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}