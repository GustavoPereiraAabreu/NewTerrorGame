using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private Transform holdPoint;

    private ItemPickup heldItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                TryPickup();
            }
            else
            {
                TryUnlockDoor();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldItem != null)
            {
                Drop();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            ItemPickup item = hit.collider.GetComponent<ItemPickup>();

            if (item == null)
                return;

            heldItem = item;
            heldItem.isHeld = true;

            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            heldItem.rb.isKinematic = true;
            heldItem.col.enabled = false;

            if (heldItem.outline != null)
                heldItem.outline.enabled = false;
        }
    }

    void TryUnlockDoor()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Debug.Log("Olhando para: " + hit.collider.name);

            DoorLock door = hit.collider.GetComponent<DoorLock>();

            if (door != null)
            {
                Debug.Log("Achei o cadeado!");
                door.TryUnlock(this);
            }
        }
    }

    void Drop()
    {
        heldItem.transform.SetParent(null);

        heldItem.isHeld = false;

        heldItem.rb.isKinematic = false;
        heldItem.col.enabled = true;

        heldItem = null;
    }

    public ItemPickup GetHeldItem()
    {
        return heldItem;
    }
}