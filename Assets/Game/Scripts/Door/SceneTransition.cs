using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 1f;

    private void Awake()
    {
        // Garante que a imagem ligou e está bloqueando cliques
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.raycastTarget = true;

        // Começa completamente preto
        Color color = _fadeImage.color;
        color.a = 1f;
        _fadeImage.color = color;
    }

    private void Start()
    {
        // A nova cena já foi carregada, toca a transição de clarear a tela
        StartCoroutine(FadeIn());
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        // Ativa a imagem e o bloqueio de cliques (para o jogador não clicar em 2 portas)
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.raycastTarget = true;

        // 1. FADE OUT (Espera a tela ficar 100% preta PRIMEIRO)
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. Começa o carregamento da nova cena
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Impede que a cena seja ativada imediatamente
        operation.allowSceneActivation = false;

        // Espera a cena terminar de carregar
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // Agora sim a cena está completamente carregada e permitimos a troca
        operation.allowSceneActivation = true;
    }

    private IEnumerator FadeIn()
    {
        // Começa preto e faz o Fade para transparente
        yield return StartCoroutine(Fade(1f, 0f));

        // A MÁGICA QUE SALVA SEU MENU: 
        // Quando a tela ficar transparente, desativamos o vidro bloqueador!
        _fadeImage.raycastTarget = false;
        _fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = _fadeImage.color;

        while (timer < _fadeDuration)
        {
            // Usamos unscaledDeltaTime no lugar de deltaTime.
            // Assim, mesmo se o jogo der lag ou estiver pausado, o fade funciona suave!
            timer += Time.unscaledDeltaTime;

            float progress = timer / _fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, progress);
            _fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        _fadeImage.color = color;
    }
}