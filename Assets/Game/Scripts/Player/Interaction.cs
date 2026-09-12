using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 3f;
    private Camera MainCamera;

    // Agora existe APENAS a variável da Interface. Sumiu o "TargetDoor".
    private Iinteractable Target;

    void Start()
    {
        MainCamera = Camera.main;
    }

    void Update()
    {
        if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, _interactionRange))
        {
            Debug.Log("O raio bateu em algo com interface: " + hit.collider.gameObject.name);
            // =======================================================
            // SISTEMA ÚNICO E UNIVERSAL (Sem tags, sem saber o que é)
            // =======================================================
            if (hit.collider.TryGetComponent(out Iinteractable interactable))
            {
                if (Target == interactable) return;

                Target?.HideOutline(); // Apaga o anterior
                Target = interactable; // Salva o novo
                Target.ShowOutline();  // Acende o novo
            }
            else
            {
                LimparAlvo();
            }
        }
        else
        {
            LimparAlvo();
        }
    }

    public void OnInteract(InputValue value)
    {
        // Só tenta executar se tiver um alvo
        Target?.Interact();
    }

    private void LimparAlvo()
    {
        if (Target != null)
        {
            Target.HideOutline();
            Target = null;
        }
    }
}