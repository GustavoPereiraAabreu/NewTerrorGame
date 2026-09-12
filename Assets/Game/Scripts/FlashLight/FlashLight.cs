using UnityEngine;

public class FlashLight : MonoBehaviour, Iinteractable
{
    private Outline _Outline;

    void Awake()
    {
        _Outline = GetComponentInChildren<Outline>();
        _Outline.enabled = false;
    }

    public void Interact()
    {
        // 1. Avisa o script do jogador que a lanterna foi destravada
        PlayerFlashlight.Instance.UnlockFlashlight();

        // 2. Destrói a lanterna do chão
        Destroy(gameObject);
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