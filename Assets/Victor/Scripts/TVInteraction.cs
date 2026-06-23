using System.Collections;
using UnityEngine;

public class TVInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private Rigidbody playerRb;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 2f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private bool watchingTV;

    private void Update()
    {
        if (watchingTV && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTV();
        }
    }

    public void EnterTV()
    {
        if (watchingTV) return;

        watchingTV = true;

        // salva posição
        originalPos = playerCamera.position;
        originalRot = playerCamera.rotation;

        // desliga controle
        playerController.playerCanMove = false;
        playerController.cameraCanMove = false;

        // zera física (ESSENCIAL)
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // cursor travado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // desliga outline da TV
        Outline outline = GetComponentInChildren<Outline>();
        if (outline != null)
            outline.enabled = false;

        StartCoroutine(MoveCamera(cameraPoint.position, cameraPoint.rotation));
    }

    public void ExitTV()
    {
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        yield return MoveCamera(originalPos, originalRot);

        playerController.playerCanMove = true;
        playerController.cameraCanMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        watchingTV = false;
    }

    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        float t = 0f;

        Vector3 startPos = playerCamera.position;
        Quaternion startRot = playerCamera.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            playerCamera.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerCamera.position = targetPos;
        playerCamera.rotation = targetRot;
    }
}