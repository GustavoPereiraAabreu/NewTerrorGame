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

    [Header("Patrol Points Para Ativar")]
    [SerializeField] private GameObject[] patrolPoints;

    public void TryUnlock(PlayerPickup player)
    {
        ItemPickup heldItem = player.GetHeldItem();

        if (heldItem == null)
            return;

        if (heldItem.itemID != requiredItemID)
            return;

        doorAnimator.SetTrigger("Open");

        if (interactionText != null)
            interactionText.SetActive(false);

        if (padlock != null)
            Destroy(padlock);

        foreach (GameObject point in patrolPoints)
        {
            if (point != null)
                point.SetActive(true);
        }

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