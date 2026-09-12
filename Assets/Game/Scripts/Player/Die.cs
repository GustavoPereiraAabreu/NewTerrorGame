using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _cameraTravada;
    [SerializeField] private GameObject _cameraMorte;
    [SerializeField] private Transform _pivoDaMorte;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cameraTravada.SetActive(false);
            _cameraMorte.SetActive(true);
            _pivoDaMorte.position = _cameraTravada.transform.position;
            _pivoDaMorte.rotation = _cameraTravada.transform.rotation;
            _animator.Play("Caindo");
            Invoke("Morreu", 3f);
        }
    }
    public void Morreu()
    {
        _panel.SetActive(true);
    }

}
