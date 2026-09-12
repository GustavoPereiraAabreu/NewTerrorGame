using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string npcName;
    [SerializeField] private string[] dialogues;

    private int currentDialogueIndex;

    public string GetNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            return dialogues[currentDialogueIndex++];
        }

        currentDialogueIndex = 0;
        return null;
    }
}