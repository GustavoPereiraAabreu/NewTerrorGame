using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlight : MonoBehaviour
{
    public static PlayerFlashlight Instance { get; private set; }
    public GameObject _spotlight;
    public bool _hasFlashlight = false;

    private void Awake()
    {
        Instance = this;

        // Garante que o jogo comece com a luz apagada
        if (_spotlight != null)
            _spotlight.SetActive(false);
    }

    public void UnlockFlashlight()
    {
        _hasFlashlight = true; // Agora o jogador possui a lanterna!

        if (_spotlight != null)
        {
            _spotlight.SetActive(true); // Já acende a luz na hora que pega
        }
    }

    public void OnFlashlight(InputValue value)
    {
        // Se o jogador não tiver pego a lanterna ainda, não faz nada
        if (!_hasFlashlight || _spotlight == null)
            return;

        // Inverte o estado da luz (se tá ligada, desliga; se tá desligada, liga)
        _spotlight.SetActive(!_spotlight.activeSelf);
    }
}