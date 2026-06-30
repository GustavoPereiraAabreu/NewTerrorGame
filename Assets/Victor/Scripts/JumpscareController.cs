using UnityEngine;

public class JumpscareController : MonoBehaviour
{
    [Header("Player")]
    public Transform playerCamera;
    public Rigidbody playerRb;
    public MonoBehaviour[] playerScripts;

    [Header("Enemy")]
    public GameObject enemy;
    public EnemyAI enemyAIScript;
    public Transform headTarget;
    public float distanceFromCamera = 1.5f;

    [Header("UI & Audio")]
    public GameObject deathScreen;
    public AudioSource sound;
    public float deathScreenDelay = 0.8f;

    [Header("Camera")]
    public float cameraSnapSpeed = 10f;

    bool triggered;
    bool lockCamera;

    Transform lookTarget;

    void Start()
    {
        if (deathScreen) deathScreen.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            TriggerJumpscare();
        }
    }

    void LateUpdate()
    {
        if (!lockCamera || !lookTarget)
            return;

        Vector3 dir = (lookTarget.position - playerCamera.position).normalized;
        playerCamera.rotation = Quaternion.LookRotation(dir);
    }

    public void TriggerJumpscare()
    {
        triggered = true;
        lockCamera = true;

        LockPlayer();
        FreezeEnemy(); 
        PositionEnemyInFront();

        lookTarget = headTarget;

        if (sound) sound.Play();

        Invoke(nameof(ShowDeathScreen), deathScreenDelay);
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
        else if (enemy != null)
        {
            
            enemy.SendMessage("FreezeEnemy", SendMessageOptions.DontRequireReceiver);
        }
    }

    void PositionEnemyInFront()
    {
        Vector3 forward = playerCamera.forward;
        Vector3 pos = playerCamera.position + forward * distanceFromCamera;

        if (enemy != null)
        {
            pos.y = enemy.transform.position.y;
            enemy.transform.position = pos;

            Vector3 dir = playerCamera.position - enemy.transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                enemy.transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    void ShowDeathScreen()
    {
        if (deathScreen) deathScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}