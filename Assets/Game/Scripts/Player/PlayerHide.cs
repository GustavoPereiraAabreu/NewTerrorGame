using UnityEngine;

public class PlayerHide : MonoBehaviour, Iinteractable
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _localSave;
    private bool _canHide = false;

    public void HideOutline()
    {
        
    }

    public void Interact()
    {
        print("foi chamado");
        Debug.Log("Ativou");
        _canHide = true;
    }

    public void ShowOutline()
    {

    }
    void Update()
    {
        if(_canHide == true)
        {
            _camera.position = _localSave.position;
        }
    }
}
