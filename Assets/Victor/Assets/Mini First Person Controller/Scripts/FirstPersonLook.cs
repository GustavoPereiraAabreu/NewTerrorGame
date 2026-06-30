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

        // Usa a rotação inicial definida na cena
        rotation.x = character.localEulerAngles.y;
        rotation.y = transform.localEulerAngles.x;

        // Converte 270° em -90°, por exemplo
        if (rotation.y > 180f)
            rotation.y -= 360f;
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
        if (!canLook)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotation.x += mouseX;
        rotation.y -= mouseY;

        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        // Rotação da câmera (vertical)
        transform.localRotation = Quaternion.Euler(rotation.y, 0f, 0f);

        // Rotação do personagem (horizontal)
        character.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
    }
}