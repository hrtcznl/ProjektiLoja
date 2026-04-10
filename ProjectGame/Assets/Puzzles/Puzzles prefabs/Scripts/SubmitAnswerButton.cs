using UnityEngine;

public class SubmitAnswerButton : MonoBehaviour, IInteractable
{
    public TextEntryPuzzleManager puzzleManager;

    public void Interact()
    {
        if (puzzleManager != null)
        {
            puzzleManager.SubmitAnswer();
        }
        else
        {
            Debug.LogWarning("PuzzleManager not assigned on SubmitAnswerButton.");
        }
    }
}