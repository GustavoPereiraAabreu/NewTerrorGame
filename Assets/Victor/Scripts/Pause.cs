using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    public MonoBehaviour playerCamera;

    bool paused;

    void Start() => pausePanel.SetActive(false);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;

            pausePanel.SetActive(paused);

            Time.timeScale = paused ? 0 : 1;

            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            playerCamera.enabled = !paused;
        }
    }
}