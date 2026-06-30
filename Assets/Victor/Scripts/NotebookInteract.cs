using UnityEngine;

public class NotebookInteract : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public GameObject painelNotebook;
    public GameObject interactionText;

    [Header("Scripts do Player")]
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    [Header("Interação")]
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

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteracao))
        {
            if (hit.transform == transform)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    AbrirNotebook();
                }
            }
        }
    }

    public void AbrirNotebook()
    {
        aberto = true;

        painelNotebook.SetActive(true);

        if (interactionText != null)
            interactionText.SetActive(false);

        playerMovement.enabled = false;
        playerLook.LockLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Chame essa função no botão X
    public void FecharNotebook()
    {
        aberto = false;

        painelNotebook.SetActive(false);

        playerMovement.enabled = true;
        playerLook.UnlockLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}