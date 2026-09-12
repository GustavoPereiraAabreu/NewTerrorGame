using UnityEngine;

public class JumpscareController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody playerRb;
    public MonoBehaviour[] playerScripts;

    [Header("Jumpscare Camera Setup")]
    public GameObject playerMainCamera;
    public GameObject jumpscareCamera;

    [Header("Enemy")]
    public EnemyAI enemyAIScript;

    [Header("UI & Audio")]
    public GameObject deathScreen;
    public AudioSource sound;
    public float deathScreenDelay = 0.8f;

    bool triggered;

    void Start()
    {
        Time.timeScale = 1f;

        if (deathScreen) deathScreen.SetActive(false);
        if (jumpscareCamera) jumpscareCamera.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            TriggerJumpscare();
        }
    }

    public void TriggerJumpscare()
    {
        if (triggered) return;
        triggered = true;
        if (playerMainCamera != null) playerMainCamera.SetActive(false);
        if (jumpscareCamera != null) jumpscareCamera.SetActive(true);
        LockPlayer();
        FreezeEnemy();
        if (sound) sound.Play();
        Invoke(nameof(ShowDeathScreen), deathScreenDelay);
        Time.timeScale = 1f;
    }

    void LockPlayer()
    {
        if (playerRb)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        foreach (var s in playerScripts)
        {
            if (s) s.enabled = false;
        }
    }

    void FreezeEnemy()
    {
        if (enemyAIScript != null)
        {
            enemyAIScript.FreezeEnemy();
        }
    }

    void ShowDeathScreen()
    {
        if (deathScreen) deathScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}