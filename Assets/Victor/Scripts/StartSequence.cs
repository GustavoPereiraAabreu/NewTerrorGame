using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSequence : MonoBehaviour
{
    [Header("HUD")]
    public CanvasGroup startText;

    [Header("Carro")]
    public Animator carAnimator;
    public string animationTrigger = "Start";

    [Header("Câmeras")]
    public Camera introCamera;
    public Transform playerCameraTarget;

    [Header("Player")]
    public GameObject playerController;

    [Header("Config")]
    public float textFadeDuration = 1f;
    public float carAnimationDuration = 5f;
    public float cameraMoveDuration = 2f;

    private bool started = false;

    void Start()
    {
        playerController.SetActive(false);
    }

    // Chame este método pelo botão da UI
    public void StartGame()
    {
        if (started) return;

        started = true;
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        // Fade do texto
        yield return StartCoroutine(FadeOutText());

        // Inicia animação do carro
        carAnimator.SetTrigger(animationTrigger);

        // Espera a animação terminar
        yield return new WaitForSeconds(carAnimationDuration);

        // Move câmera até o player
        yield return StartCoroutine(MoveCameraToPlayer());

        // Ativa o player
        playerController.SetActive(true);

        // Desativa a câmera de introdução
        introCamera.gameObject.SetActive(false);
    }

    IEnumerator FadeOutText()
    {
        float t = 0f;

        while (t < textFadeDuration)
        {
            t += Time.deltaTime;

            startText.alpha = Mathf.Lerp(1f, 0f, t / textFadeDuration);

            yield return null;
        }

        startText.alpha = 0f;
    }

    IEnumerator MoveCameraToPlayer()
    {
        Vector3 startPos = introCamera.transform.position;
        Quaternion startRot = introCamera.transform.rotation;

        float t = 0f;

        while (t < cameraMoveDuration)
        {
            t += Time.deltaTime;

            float progress = t / cameraMoveDuration;

            introCamera.transform.position =
                Vector3.Lerp(startPos, playerCameraTarget.position, progress);

            introCamera.transform.rotation =
                Quaternion.Slerp(startRot, playerCameraTarget.rotation, progress);

            yield return null;
        }

        introCamera.transform.position = playerCameraTarget.position;
        introCamera.transform.rotation = playerCameraTarget.rotation;
    }
}