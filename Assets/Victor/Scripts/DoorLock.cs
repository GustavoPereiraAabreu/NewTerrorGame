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

    [Header("Time System Trigger")]
    [SerializeField] private TimeManager timeManager;

    public void TryUnlock(PlayerPickup player)
    {
        ItemPickup heldItem = player.GetHeldItem();

        if (heldItem == null)
        {;
            return;
        }

        if (heldItem.itemID != requiredItemID)
        { 
            return;
        }


        doorAnimator.SetTrigger("Open");

        if (timeManager != null)
        {
            timeManager.enabled = true;
        }

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