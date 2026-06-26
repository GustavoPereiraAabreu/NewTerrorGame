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

    [Header("Cameras")]
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

    public void StartGame()
    {
        if (started) return;

        started = true;
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        yield return StartCoroutine(FadeOutText());

        carAnimator.SetTrigger(animationTrigger);

        yield return new WaitForSeconds(carAnimationDuration);

        yield return StartCoroutine(MoveCameraToPlayer());

        playerController.SetActive(true);

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