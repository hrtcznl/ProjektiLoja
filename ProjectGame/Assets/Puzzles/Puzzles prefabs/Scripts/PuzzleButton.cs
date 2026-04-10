using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteractable
{
    public int buttonID; // 0, 1, 2
    public PuzzleManager puzzleManager;

    public void Interact()
    {
        Debug.Log("Button " + buttonID + " pressed");

        if (puzzleManager != null)
        {
            puzzleManager.CheckAnswer(buttonID);
        }
        else
        {
            Debug.LogWarning("PuzzleManager not assigned!");
        }
    }
}