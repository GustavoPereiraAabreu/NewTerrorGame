using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]


    public class Door : MonoBehaviour, Iinteractable
    {
        private bool _open;
        [SerializeField] private float _smooth = 1.0f;
        [SerializeField] private float _DoorOpenAngle = -90.0f;
        [SerializeField] private float _DoorCloseAngle = 0.0f;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _openDoor, _closeDoor;
        private bool _isMove;
        private Outline _Outline;
        // sistema de abrir com chave
        [SerializeField] private bool _unlocked;

        void Start()
        {
            _Outline = GetComponentInChildren<Outline>();
            _Outline.enabled = false;
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            Quaternion target;
            if (_open)
            {
                target = Quaternion.Euler(-90, _DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * _smooth);
            }
            else
            {
                target = Quaternion.Euler(-90, _DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * _smooth);
            }
            if (_isMove && Quaternion.Angle(transform.localRotation, target) < 0.5f)
            {
                transform.localRotation = target;
                _isMove = false;
            }
        }

        public void Interact()
        {
            if (_isMove)
                return;

            _open = !_open;
            _isMove = true;

            _unlocked = false;
            _audioSource.clip = _open ? _openDoor : _closeDoor;
            _audioSource.Play();
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
}