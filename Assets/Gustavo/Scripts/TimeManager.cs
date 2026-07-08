using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float secondsPerHour = 60f;

    [Header("UI References")]
    public TMP_Text timeText;
    public TMP_Text recordingText;
    public GameObject winScreen;

    [Header("Game Lock References")]
    public Rigidbody playerRb;
    public MonoBehaviour[] playerScripts;
    public EnemyAI enemyAIScript;

    private int currentHour = 12;
    private float hourTimer;
    private bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (winScreen != null)
            winScreen.SetActive(false);

        UpdateVisualTime();
    }

    void Update()
    {
        if (gameEnded) return;

        hourTimer += Time.deltaTime;

        UpdateRecordingTimer();

        if (hourTimer >= secondsPerHour)
        {
            hourTimer = 0;
            AdvanceHour();
        }
    }

    void UpdateRecordingTimer()
    {
        if (recordingText == null) return;

        int minutes = Mathf.FloorToInt((hourTimer / secondsPerHour) * 60);

        if (minutes > 59)
            minutes = 59;

        recordingText.text = $"{currentHour:00}:{minutes:00}";
    }

    void AdvanceHour()
    {
        currentHour++;

        if (currentHour == 13)
            currentHour = 1;

        UpdateVisualTime();

        if (currentHour == 6)
            WinGame();
    }

    void UpdateVisualTime()
    {
        if (timeText != null)
            timeText.text = currentHour + " AM";
    }

    void WinGame()
    {
        gameEnded = true;

        if (enemyAIScript != null)
            enemyAIScript.FreezeEnemy();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        foreach (MonoBehaviour script in playerScripts)
        {
            if (script != null)
                script.enabled = false;
        }

        if (winScreen != null)
            winScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}