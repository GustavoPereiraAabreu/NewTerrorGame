using System.Collections;
using UnityEngine;

public class NotebookInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject painelNotebook;

    [Header("Player")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour mouseLook;

    private bool aberto = false;

    void Start()
    {
        painelNotebook.SetActive(false);
    }

    void Update()
    {
        if (!aberto && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(AbrirNotebook());
        }

        // Se o painel foi fechado por outro script ou botão
        if (aberto && !painelNotebook.activeSelf)
        {
            FecharNotebook();
        }
    }

    IEnumerator AbrirNotebook()
    {
        aberto = true;

        painelNotebook.SetActive(true);

        // Espera um frame para evitar input preso
        yield return null;

        playerMovement.enabled = false;
        mouseLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void FecharNotebook()
    {
        aberto = false;

        playerMovement.enabled = true;
        mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}