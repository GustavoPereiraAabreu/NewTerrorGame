using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private string requiredItemID;
    [SerializeField] private Animator doorAnimator;

    public void TryUnlock(PlayerPickup player)
    {
        Debug.Log("Tentou destrancar");

        ItemPickup heldItem = player.GetHeldItem();

        if (heldItem == null)
        {
            Debug.Log("Sem item");
            return;
        }

        if (heldItem.itemID != requiredItemID)
        {
            Debug.Log("Item errado: " + heldItem.itemID);
            return;
        }

        Debug.Log("Abrindo porta");

        doorAnimator.SetTrigger("Open");

        Destroy(heldItem.gameObject);

        Destroy(gameObject);
    }
}