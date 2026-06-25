using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] Transform character;

    public float sensitivity = 2f;

    Vector2 rotation;

    public bool canLook = true;

    void Reset()
    {
        character = GetComponentInParent<Transform>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LockLook()
    {
        canLook = false;
    }

    public void UnlockLook()
    {
        canLook = true;
    }

    void Update()
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotation.x += mouseX;
        rotation.y -= mouseY;

        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotation.y, 0f, 0f);
        character.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
    }
}