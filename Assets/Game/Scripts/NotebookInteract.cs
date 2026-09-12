using UnityEngine;
using UnityEngine.UI;

public class NotebookInteract : MonoBehaviour, Iinteractable
{
    [SerializeField] private GameObject _panelNotebook;
    private Outline _Outline;
    private bool _open = false;
    [SerializeField] private MonoBehaviour FirstPersonController; 

    void Start()
    {
        _Outline = GetComponentInChildren<Outline>();
        _Outline.enabled = false;
        _panelNotebook.SetActive(false);
        _open = false;
    }

    public void Interact()
    {
        // Inverte o estado atual (Se era false, vira true. Se era true, vira false)
        _open = !_open;

        if (_open)
        {
            _panelNotebook.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (FirstPersonController != null) FirstPersonController.enabled = false;
        }
        else
        {
            _panelNotebook.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (FirstPersonController != null) FirstPersonController.enabled = true;
        }
    }
    

    public void ShowOutline()
    {
        if (_Outline != null) _Outline.enabled = true;
    }

    public void HideOutline()
    {
        if (_Outline != null) _Outline.enabled = false;
    }
}