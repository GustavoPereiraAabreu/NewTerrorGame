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

        if (winScreen) winScreen.SetActive(false);
        UpdateVisualTime();
    }

    void Update()
    {
        if (gameEnded) return;

        hourTimer += Time.deltaTime;

        UpdateRecordingTimer();

        if (hourTimer >= secondsPerHour)
        {
            hourTimer = 0f;
            AdvanceHour();
        }
    }

    void UpdateRecordingTimer()
    {
        if (recordingText != null)
        {
            float progressOfCurrentHour = hourTimer / secondsPerHour;
            int minutes = Mathf.FloorToInt(progressOfCurrentHour * 60f);

            if (minutes > 59) minutes = 59;

            recordingText.text = $"{currentHour:00}:{minutes:00}";
        }
    }

    void AdvanceHour()
    {
        if (currentHour == 12)
        {
            currentHour = 1;
        }
        else
        {
            currentHour++;
        }

        UpdateVisualTime();

        if (currentHour == 6)
        {
            WinGame();
        }
    }

    void UpdateVisualTime()
    {
        if (timeText != null)
        {
            timeText.text = currentHour.ToString() + " AM";
        }
    }

    void WinGame()
    {
        gameEnded = true;

        if (enemyAIScript != null)
        {
            enemyAIScript.FreezeEnemy();
        }

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

        if (winScreen) winScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}