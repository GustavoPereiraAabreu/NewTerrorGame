using UnityEngine;
using UnityEngine.SceneManagement;
public class KeyCode : MonoBehaviour, Iinteractable
{
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
       
    }

    public void ShowOutline()
    {
        if (_Outline != null) _Outline.enabled = true;
    }
}
