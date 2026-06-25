using UnityEngine;

public class NotebookInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject painelNotebook;

    [Header("Player")]
    public MonoBehaviour playerMovement;
    public FirstPersonLook mouseLook;

    [Header("Interação")]
    public Camera playerCamera;
    public float distanciaInteracao = 3f;

    private bool aberto;

    void Start()
    {
        FecharNotebook();
    }

    void Update()
    {
        if (aberto) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        bool olhando = Physics.Raycast(ray, out hit, distanciaInteracao)
                       && hit.transform == transform;

        if (olhando && Input.GetKeyDown(KeyCode.E))
        {
            AbrirNotebook();
        }
    }

    public void AbrirNotebook()
    {
        aberto = true;

        painelNotebook.SetActive(true);

        if (playerMovement) playerMovement.enabled = false;
        if (mouseLook) mouseLook.LockLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FecharNotebook()
    {
        aberto = false;

        painelNotebook.SetActive(false);

        if (playerMovement) playerMovement.enabled = true;
        if (mouseLook) mouseLook.UnlockLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}