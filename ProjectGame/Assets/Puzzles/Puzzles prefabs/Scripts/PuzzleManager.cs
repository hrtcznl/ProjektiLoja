using UnityEngine;

[System.Serializable]
public class PuzzleOption
{
    public Texture2D image;
    public int correctButtonID;
}

public class PuzzleManager : MonoBehaviour
{
    public Renderer displayRenderer;
    public PuzzleOption[] puzzleOptions;

    private int currentCorrectButtonID;
    private int currentIndex;
    private bool puzzleSolved = false;

    void Start()
    {
        Debug.Log("PuzzleManager started");

        if (displayRenderer == null)
        {
            Debug.LogError("Display Renderer is NOT assigned");
            return;
        }

        if (puzzleOptions == null || puzzleOptions.Length == 0)
        {
            Debug.LogError("No puzzle options assigned");
            return;
        }

        ShowRandomPuzzle();
    }

    void ShowRandomPuzzle()
    {
        currentIndex = Random.Range(0, puzzleOptions.Length);

        Texture2D tex = puzzleOptions[currentIndex].image;

        if (tex == null)
        {
            Debug.LogError("Image is NULL");
            return;
        }

        Material mat = displayRenderer.material;

        // Works for both Standard and URP shaders
        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", tex);

        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", tex);

        currentCorrectButtonID = puzzleOptions[currentIndex].correctButtonID;

        Debug.Log("Correct button is: " + currentCorrectButtonID);
    }

    public void CheckAnswer(int pressedButtonID)
    {
        if (puzzleSolved)
        {
            Debug.Log("Puzzle already solved.");
            return;
        }

        if (pressedButtonID == currentCorrectButtonID)
        {
            Debug.Log("You are right!");
            puzzleSolved = true;
        }
        else
        {
            Debug.Log("Wrong answer!");
        }
    }
}