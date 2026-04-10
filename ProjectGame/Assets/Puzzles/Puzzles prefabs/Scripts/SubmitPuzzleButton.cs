using UnityEngine;

public class SubmitPuzzleButton : MonoBehaviour, IInteractable
{
    public ImageTextPuzzleManager puzzleManager;

    public void Interact()
    {
        if (puzzleManager != null)
        {
            puzzleManager.CheckAnswer();
        }
        else
        {
            Debug.LogWarning("PuzzleManager not assigned.");
        }
    }
}