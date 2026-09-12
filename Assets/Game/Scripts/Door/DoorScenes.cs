using UnityEngine;
public class DoorScenes : MonoBehaviour, Iinteractable
{
    [SerializeField] private string _name;
    [SerializeField] private SceneTransition _sceneTransition;
    public AudioSource audioSource;
    public AudioClip doorSound;
    private Outline _Outline;

    private void Start()
    {
        _Outline = GetComponentInChildren<Outline>();
        _Outline.enabled = false;
    }

    public void HideOutline()
    {
        if (_Outline != null) _Outline.enabled = false;
    }

    public void Interact()
    {
        _sceneTransition.ChangeScene(_name);
        audioSource.PlayOneShot(doorSound);
    }

    public void ShowOutline()
    {
        if (_Outline != null) _Outline.enabled = true;
    }
}