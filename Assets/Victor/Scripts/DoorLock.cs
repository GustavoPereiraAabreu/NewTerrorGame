using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private string requiredItemID;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject interactionText;

    [Header("Objetos da Porta")]
    [SerializeField] private GameObject padlock;
    [SerializeField] private NavMeshObstacle navObstacle;

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

        if (interactionText != null)
            interactionText.SetActive(false);

        if (padlock != null)
            Destroy(padlock);

        Destroy(heldItem.gameObject);

        StartCoroutine(RemoverObstaculo());

        Destroy(this);
    }

    private IEnumerator RemoverObstaculo()
    {
        yield return new WaitForSeconds(1f);

        if (navObstacle != null)
            navObstacle.enabled = false;
    }
}