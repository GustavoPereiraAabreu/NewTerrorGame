using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinTrigger : MonoBehaviour
{
    public GameObject winUI;
    public float fadeTime = 1.5f;

    private Graphic[] graphics;
    private bool activated = false;

    void Start()
    {
        winUI.SetActive(true);
        graphics = winUI.GetComponentsInChildren<Graphic>();

        SetAlpha(0f);

        winUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            Time.timeScale = 0f;

            winUI.SetActive(true);
            StartCoroutine(FadeInUI());
        }
    }

    IEnumerator FadeInUI()
    {
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;

            float v = t / fadeTime;
            SetAlpha(v);

            yield return null;
        }

        SetAlpha(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SetAlpha(float alpha)
    {
        foreach (Graphic g in graphics)
        {
            if (g == null) continue;

            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
    }
}