using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteract : MonoBehaviour
{
    public string sceneName;
    public AudioSource audioSource;
    public AudioClip doorSound;
    public float distance = 3f;

    void Update()
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.gameObject == gameObject && Input.GetKeyDown(KeyCode.E))
            {
                if (audioSource && doorSound)
                    audioSource.PlayOneShot(doorSound);

                Invoke("LoadScene", 1.5f);
            }
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
