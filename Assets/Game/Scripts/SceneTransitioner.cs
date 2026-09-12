using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;

    [Header("Settings Panel")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeSpeed = 1.5f;

    [Tooltip("Fade Duration")]
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ATUALIZADO: Usando o método moderno e otimizado da Unity
        if (fadeGroup == null)
        {
            fadeGroup = Object.FindAnyObjectByType<CanvasGroup>();
        }

        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            fadeGroup.blocksRaycasts = true;
            StartCoroutine(FadeOut());
        }
    }

    private void Start()
    {
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
        float elapsedTime = 0f;
        float startAlpha = fadeGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        if (fadeGroup == null)
        {
            fadeGroup = Object.FindAnyObjectByType<CanvasGroup>();
        }

        if (fadeGroup != null)
        {
            fadeGroup.blocksRaycasts = true;

            float elapsedTime = 0f;
            float startAlpha = fadeGroup.alpha;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            fadeGroup.alpha = 1f;
        }

        fadeGroup = null;
        SceneManager.LoadScene(sceneName);
    }
}