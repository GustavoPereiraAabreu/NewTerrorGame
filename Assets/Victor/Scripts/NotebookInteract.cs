using System.Collections;
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

    private bool aberto = false;

    void Start()
    {
        if (painelNotebook != null)
            painelNotebook.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        bool olhandoParaNotebook = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao))
        {
            if (hit.transform == transform)
                olhandoParaNotebook = true;
        }

        if (!aberto && olhandoParaNotebook && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(AbrirNotebook());
        }

        if (aberto && painelNotebook != null && !painelNotebook.activeSelf)
        {
            FecharNotebook();
        }
    }

    IEnumerator AbrirNotebook()
    {
        aberto = true;

        if (painelNotebook != null)
            painelNotebook.SetActive(true);

        yield return null;

        if (mouseLook != null)
            mouseLook.LockLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void FecharNotebook()
    {
        aberto = false;

        if (mouseLook != null)
            mouseLook.UnlockLook();

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}