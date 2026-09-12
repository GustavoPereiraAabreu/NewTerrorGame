using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerHands : MonoBehaviour
{
    // Singleton para os itens acharem a mão do jogador facilmente
    public static PlayerHands Instance { get; private set; }
    public Transform _handPos;
    private Transform _objects;
    private AudioSource _audioSource;
    public AudioClip _audioClipCollect;
    public AudioClip _audioClipDrop;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }
    public void TakeObject(Transform ObjectToGrab)
    {
        if (_objects != null)
            return;

        _objects = ObjectToGrab;

        Rigidbody _rb = _objects.GetComponent<Rigidbody>();
        Collider col = _objects.GetComponent<Collider>();


        _rb.isKinematic = true;
        _rb.useGravity = false;
        _objects.transform.SetParent(_handPos);
        _objects.transform.localPosition = Vector3.zero;
        _objects.transform.localRotation = Quaternion.Euler(0, 0, 0f);

        if (col != null)
        {
            col.enabled = false;
        }

        if (_audioClipCollect != null)
        {
            _audioSource.PlayOneShot(_audioClipCollect);
        }
    }
    public void OnDrop(InputValue value)
    {
        if (_objects == null)
            return;
        _objects.SetParent(null);

        Rigidbody rb = _objects.transform.GetComponent<Rigidbody>();
        Collider col = _objects.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        if (col != null)
        {
            col.enabled = true;
        }
        if (_audioClipDrop != null)
        {
            _audioSource.PlayOneShot(_audioClipDrop);
        }

        _objects = null;
    }

    public void OnItemActivation(InputValue value)
    {
        if (_objects == null)
            return;
        if (!_objects.TryGetComponent(out Iinteractable iinteractable))
            return;
        iinteractable.Interact();
    }

}
