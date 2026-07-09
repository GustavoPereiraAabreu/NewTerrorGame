using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] private Transform character;
    [SerializeField] private float sensitivity = 2f;

    private Vector2 rotation;

    public bool canLook = true;

    void Reset()
    {
        if (transform.parent != null)
            character = transform.parent;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SyncRotation();
    }

    void OnEnable()
    {
        SyncRotation();
    }

    void SyncRotation()
    {
        if (character == null)
            return;

        rotation.x = character.localEulerAngles.y;
        rotation.y = transform.localEulerAngles.x;

        if (rotation.y > 180f)
            rotation.y -= 360f;
    }

    public void LockLook()
    {
        canLook = false;
    }

    public void UnlockLook()
    {
        SyncRotation();
        canLook = true;
    }

    void Update()
    {
        if (!canLook)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        rotation.x += mouseX;
        rotation.y -= mouseY;

        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotation.y, 0f, 0f);
        character.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
    }
}