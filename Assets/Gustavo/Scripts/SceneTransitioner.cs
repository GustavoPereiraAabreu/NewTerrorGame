using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;

    [Header("Configuração do Painel")]
    // Arraste o Canvas Group do seu Painel aqui
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeSpeed = 1.5f;

    private void Awake()
    {
        // Padrão Singleton: Garante que só exista um gerenciador no jogo todo
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Garante que o painel comece cobrindo a tela e clareie (Fade Out)
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            StartCoroutine(FadeOut());
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(PerformTransition(sceneName));
    }

    private IEnumerator FadeOut()
    {
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        fadeGroup.blocksRaycasts = false; // Permite clicar nas coisas após o fade sumir
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        fadeGroup.blocksRaycasts = true; // Bloqueia cliques durante o fade

        // FADE IN: Escurece a tela (Alpha vai para 1)
        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Carrega a nova cena
        SceneManager.LoadScene(sceneName);

        // Aguarda um frame para garantir que a cena carregou antes de clarear
        yield return null;

        // FADE OUT: Clareia a tela na nova cena (Alpha vai para 0)
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        fadeGroup.blocksRaycasts = false;
    }
}