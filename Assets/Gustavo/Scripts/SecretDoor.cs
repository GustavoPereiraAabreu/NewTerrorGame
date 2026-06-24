using UnityEngine;

public class SecretDoor : MonoBehaviour
{

    [Header("Interaction UI")]
    [SerializeField] private GameObject interactionText; // The "Right Click to Hold" UI object

    [Header("Hold Settings")]
    [SerializeField] private float pullSpeed = 10f; // How fast the object moves towards the player's hands

    private Transform holdPoint; // The empty object point in front of the player's camera
    private Rigidbody rb;
    private bool isPlayerNearby = false;
    private bool isBeingHeld = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensures the object has a Rigidbody attached
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    private void Update()
    {
        // If the player is nearby and RIGHT CLICKS to pick it up
        if (isPlayerNearby && Input.GetMouseButtonDown(1) && !isBeingHeld)
        {
            Pickup();
        }
        // If already holding and RIGHT CLICKS again to drop it
        else if (isBeingHeld && Input.GetMouseButtonDown(1))
        {
            Drop();
        }
    }

    private void FixedUpdate()
    {
        // If being held, smoothly move the object towards the point in front of the camera
        if (isBeingHeld && holdPoint != null)
        {
            Vector3 direction = holdPoint.position - transform.position;
            rb.linearVelocity = direction * pullSpeed; // For older Unity versions, use: rb.velocity = direction * pullSpeed;
        }
    }

    private void Pickup()
    {
        isBeingHeld = true;
        rb.useGravity = false; // Turns off gravity while holding
        rb.angularDamping = 5f; // Prevents the object from spinning wildly (Older Unity: rb.angularDrag = 5f;)

        // Hides the interaction text while holding
        if (interactionText != null) interactionText.SetActive(false);
    }

    private void Drop()
    {
        isBeingHeld = false;
        rb.useGravity = true; // Returns gravity so it falls back down
        rb.angularDamping = 0.05f; // Restores default angular rotation damping
    }

    // Assigns the hold point when the Player enters the interaction zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // Finds the point where the object will be held in front of the camera
            if (holdPoint == null)
            {
                Camera playerCamera = other.GetComponentInChildren<Camera>();
                if (playerCamera != null)
                {
                    Transform cameraTransform = playerCamera.transform;
                    holdPoint = cameraTransform.Find("HoldPointObject");

                    // If it doesn't exist, automatically creates one in front of the camera
                    if (holdPoint == null)
                    {
                        GameObject newHoldPoint = new GameObject("HoldPointObject");
                        newHoldPoint.transform.SetParent(cameraTransform);
                        newHoldPoint.transform.localPosition = new Vector3(0f, 0f, 2.5f); // 2.5 meters in front of the camera
                        holdPoint = newHoldPoint.transform;
                    }
                }
            }

            if (interactionText != null && !isBeingHeld)
            {
                interactionText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }

            // Forces the object to drop if the player runs too far away
            if (isBeingHeld)
            {
                Drop();
            }
        }
    }
}