using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private string requiredItemID;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject interactionText;

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

        // Esconde o texto de interação
        if (interactionText != null)
            interactionText.SetActive(false);

        Destroy(heldItem.gameObject);

        Destroy(gameObject);
    }
}