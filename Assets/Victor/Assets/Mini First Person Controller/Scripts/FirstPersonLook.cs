using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] Transform character;

    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    public bool canLook = true;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LockLook()
    {
        canLook = false;
    }

    public void UnlockLook()
    {
        canLook = true;
        frameVelocity = Vector2.zero;
    }

    void Update()
    {
        if (!canLook)
            return;

        Vector2 mouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        Vector2 rawFrameVelocity =
            Vector2.Scale(mouseDelta, Vector2.one * sensitivity);

        frameVelocity = Vector2.Lerp(
            frameVelocity,
            rawFrameVelocity,
            Time.deltaTime * 15f
        );

        velocity += frameVelocity;

        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);

        transform.localRotation =
            Quaternion.AngleAxis(-velocity.y, Vector3.right);

        character.localRotation =
            Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}