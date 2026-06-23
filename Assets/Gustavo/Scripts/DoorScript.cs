using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class DoorScript : MonoBehaviour
{

    [Header("UI Interact")]
    [SerializeField] private GameObject txtInteract; // O objeto de texto "Aperte E para interagir"

    [Header("Scene Transition Settings")]
    [SerializeField] private string nameScene; // Nome exato da cena para onde o player vai

    [Header("Audio Settings")]
    [SerializeField] private AudioSource DoorSound; // Componente AudioSource da porta

    [SerializeField] private bool onInteract = false;

    private void Start()
    {
        // Garante que o texto comece desativado
        if (txtInteract != null)
        {
            txtInteract.SetActive(false);
        }

        if (DoorSound == null)
        {
            DoorSound = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Se o player está na área e apertou E
        if (onInteract && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        onInteract = false; // Evita que o player aperte o botão várias vezes
        
        // O SOM SÓ É TOCADO AQUI (Apenas quando o E é pressionado)
        if (DoorSound != null && DoorSound.clip != null)
        {
            DoorSound.Play();
        }

        // Esconde o texto da tela
        if (txtInteract != null)
        {
            txtInteract.SetActive(false);
        }

        // Chama o Singleton para fazer o Fade e mudar de cena
        if (SceneTransitioner.Instance != null)
        {
            SceneTransitioner.Instance.FadeToScene(nameScene);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nameScene);
        }
    }

    // Detecta o Player entrando no Collider da Porta
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onInteract = true;
            if (txtInteract != null)
            {
                txtInteract.SetActive(true);
            }
        }
    }

    // Detecta o Player saindo do Collider da Porta
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onInteract = false;
            if (txtInteract != null)
            {
                txtInteract.SetActive(false);
            }
        }
    }
}