using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerCollect : MonoBehaviour
{
    public float _interactDistance = 3f;
    public Transform _handPos;
    public Camera _mainCamera;
    public LayerMask LayerMask;
    public Transform _objects;
    private AudioSource _audioSource;
    public AudioClip _audioClipCollect;
    public AudioClip _audioClipDrop;

    private void Awake()
    {
        _objects = null;
        _audioSource = GetComponent<AudioSource>();
    }
    public void OnCollect(InputValue value)
    {
        Interact();
    }
    public void OnDrop(InputValue value)
    {
        Drop();
    }

    public void Interact()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, _interactDistance, LayerMask))
        {
            if (_objects != null)
                return;

            _objects = hit.transform;

            Rigidbody _rb = hit.collider.attachedRigidbody;
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
    }
    public void Drop()
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
