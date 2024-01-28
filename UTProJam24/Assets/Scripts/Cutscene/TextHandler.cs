using System.Collections;
using UnityEngine;

public class TextHandler : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueAsset inventoryFullText;
    [SerializeField] private DialogueAsset itemAlreadyExistsText;
    private bool currentlyDisplayingTop = false;
    private void OnEnable()
    {
        InventorySystem.InventoryFull += ShowFullInventoryText;
        InventorySystem.ItemAlreadyInInventory += ShowItemExistsText;
    }

    private void OnDisable()
    {
        InventorySystem.InventoryFull -= ShowFullInventoryText;
    }

    private void ShowFullInventoryText()
    {
        if (currentlyDisplayingTop) return;
        StartCoroutine(ShowTopDialogueWithTimer(inventoryFullText, 2f));
    }

    private void ShowItemExistsText()
    {
        if (currentlyDisplayingTop) return;
        StartCoroutine(ShowTopDialogueWithTimer(itemAlreadyExistsText, 2f));
    }

    IEnumerator ShowTopDialogueWithTimer(DialogueAsset dialogue, float duration)
    {
        currentlyDisplayingTop = true;
        dialogueManager.StartDialogue(dialogue, DialogueManager.DialogueType.Top);
        yield return new WaitForSeconds(duration);
        dialogueManager.ContinueTopDialogue();
        dialogueManager.ResetTopDialogue();
        currentlyDisplayingTop = false;
    }
}
