using UnityEngine;
using System.Collections;

public class FadeOutText : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Tempo do fade-in")]
    [SerializeField] private float fadeInTime = 1f;

    [Header("Tempo visível na tela")]
    [SerializeField] private float visibleTime = 3f;

    [Header("Tempo do fade-out")]
    [SerializeField] private float fadeOutTime = 1f;

    IEnumerator Start()
    {
        canvasGroup.alpha = 0f;

        // Fade In
        float t = 0f;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeInTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Espera
        yield return new WaitForSeconds(visibleTime);

        // Fade Out
        t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeOutTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}