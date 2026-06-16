using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private NPC npcAtual;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && npcAtual != null)
        {
            string fala = npcAtual.GetNextDialogue();

            if (fala != null)
            {
                Debug.Log(fala);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NPC npc = other.GetComponent<NPC>();

        if (npc != null)
        {
            npcAtual = npc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NPC npc = other.GetComponent<NPC>();

        if (npc == npcAtual)
        {
            npcAtual = null;
        }
    }
}