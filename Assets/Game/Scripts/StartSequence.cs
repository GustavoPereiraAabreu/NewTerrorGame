using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSequence : MonoBehaviour
{
    [Header("HUD")]
    public CanvasGroup[] _startText; // Lembre-se de colocar o Canv_Main aqui

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        // 1. Faz a transição do Alpha sumindo suavemente
        yield return StartCoroutine(FadeOutText());

        // 2. A MÁGICA: Agora podemos desligar TODAS as telas da lista sem medo,
        // porque este script está rodando na Câmera!
        foreach (CanvasGroup tela in _startText)
        {
            if (tela != null)
            {
                tela.gameObject.SetActive(false);
            }
        }

        // 3. O carro anima
        if (carAnimator != null) carAnimator.SetTrigger(animationTrigger);
        yield return new WaitForSeconds(carAnimationDuration);

        // 4. A câmera move
        yield return StartCoroutine(MoveCameraToPlayer());

        // 5. Troca para o player real e desativa esta câmera do menu (o script finaliza aqui)
        playerController.SetActive(true);
        introCamera.gameObject.SetActive(false);

    }

    IEnumerator FadeOutText()
    {
        float t = 0f;

        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            float progresso = t / textFadeDuration;

            // Passa por cada tela na sua lista e diminui a transparência
            foreach (CanvasGroup tela in _startText)
            {
                if (tela != null)
                {
                    tela.alpha = Mathf.Lerp(1f, 0f, progresso);
                }
            }

            yield return null;
        }


        // Garante que todas as telas fiquem no 0 no final
        foreach (CanvasGroup tela in _startText)
        {
            if (tela != null)
            {
                tela.alpha = 0f;
            }
        }
    }

        IEnumerator MoveCameraToPlayer()
        {
            // Destrava o Animator do asset SlimUI para a câmera se mover livremente
            Animator camAnimator = introCamera.GetComponent<Animator>();
            if (camAnimator != null)
            {
                camAnimator.enabled = false;
            }

            Vector3 startPos = introCamera.transform.position;
            Quaternion startRot = introCamera.transform.rotation;

            float t = 0f;

            while (t < cameraMoveDuration)
            {
                t += Time.deltaTime;
                float progress = t / cameraMoveDuration;

                introCamera.transform.position = Vector3.Lerp(startPos, playerCameraTarget.position, progress);
                introCamera.transform.rotation = Quaternion.Slerp(startRot, playerCameraTarget.rotation, progress);

                yield return null;
            }

            introCamera.transform.position = playerCameraTarget.position;
            introCamera.transform.rotation = playerCameraTarget.rotation;
        }
}
