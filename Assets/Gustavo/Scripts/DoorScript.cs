using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class DoorScript : MonoBehaviour
{

    [Header("UI de Interação")]
    [SerializeField] private GameObject txtInteragir; // O objeto de texto "Aperte E para interagir"

    [Header("Configurações da Transição")]
    [SerializeField] private string nomeDaCena; // Nome exato da cena para onde o player vai

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource somPorta; // Componente AudioSource da porta

    [SerializeField]private bool podeInteragir = false;

    private void Start()
    {
        // Garante que o texto comece desativado
        if (txtInteragir != null)
        {
            txtInteragir.SetActive(false);
        }

        if (somPorta == null)
        {
            somPorta = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Se o player está na área e apertou E
        if (podeInteragir && Input.GetKeyDown(KeyCode.E))
        {
            Interagir();
        }
    }

    private void Interagir()
    {
        podeInteragir = false; // Evita que o player aperte o botão várias vezes
        
        // O SOM SÓ É TOCADO AQUI (Apenas quando o E é pressionado)
        if (somPorta != null && somPorta.clip != null)
        {
            somPorta.Play();
        }

        // Esconde o texto da tela
        if (txtInteragir != null)
        {
            txtInteragir.SetActive(false);
        }

        // Chama o Singleton para fazer o Fade e mudar de cena
        if (SceneTransitioner.Instance != null)
        {
            SceneTransitioner.Instance.FadeToScene(nomeDaCena);
        }
        else
        {
            Debug.LogWarning("SceneTransitioner não foi encontrado na cena!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nomeDaCena);
        }
    }

    // Detecta o Player entrando no Collider da Porta
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
            if (txtInteragir != null)
            {
                txtInteragir.SetActive(true);
            }
        }
    }

    // Detecta o Player saindo do Collider da Porta
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
            if (txtInteragir != null)
            {
                txtInteragir.SetActive(false);
            }
        }
    }
}