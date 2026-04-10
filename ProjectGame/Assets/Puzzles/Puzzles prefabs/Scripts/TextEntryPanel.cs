using UnityEngine;

public class TextEntryPanel : MonoBehaviour, IInteractable
{
    public TextEntryPuzzleManager puzzleManager;

    public void Interact()
    {
        if (puzzleManager != null)
        {
            puzzleManager.StartTyping();
        }
        else
        {
            Debug.LogWarning("PuzzleManager not assigned on TextEntryPanel.");
        }
    }
}