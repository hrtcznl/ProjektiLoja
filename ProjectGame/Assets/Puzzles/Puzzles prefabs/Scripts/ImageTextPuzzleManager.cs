using UnityEngine;
using TMPro;

[System.Serializable]
public class TextPuzzleOption
{
    public Texture2D image;
    public string correctAnswer;
}

public class ImageTextPuzzleManager : MonoBehaviour
{
    public Renderer displayRenderer;
    public TextPuzzleOption[] puzzleOptions;

    public TMP_InputField answerInputField;
    public TMP_Text resultText;

    private string currentCorrectAnswer;
    private bool puzzleSolved = false;

    void Start()
    {
        if (displayRenderer == null)
        {
            Debug.LogError("Display Renderer is not assigned.");
            return;
        }

        if (puzzleOptions == null || puzzleOptions.Length == 0)
        {
            Debug.LogError("No puzzle options assigned.");
            return;
        }

        if (answerInputField == null)
        {
            Debug.LogError("Answer Input Field is not assigned.");
            return;
        }

        ShowRandomPuzzle();

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    void ShowRandomPuzzle()
    {
        int randomIndex = Random.Range(0, puzzleOptions.Length);
        TextPuzzleOption selectedPuzzle = puzzleOptions[randomIndex];

        if (selectedPuzzle.image == null)
        {
            Debug.LogError("Selected puzzle image is null.");
            return;
        }

        Material mat = displayRenderer.material;

        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", selectedPuzzle.image);

        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", selectedPuzzle.image);

        currentCorrectAnswer = selectedPuzzle.correctAnswer.Trim().ToLower();

        Debug.Log("Correct answer is: " + currentCorrectAnswer);
    }

    public void CheckAnswer()
    {
        if (puzzleSolved)
        {
            if (resultText != null)
                resultText.text = "Puzzle already solved.";

            Debug.Log("Puzzle already solved.");
            return;
        }

        string playerAnswer = answerInputField.text.Trim().ToLower();

        if (playerAnswer == currentCorrectAnswer)
        {
            puzzleSolved = true;

            if (resultText != null)
                resultText.text = "You are right!";

            Debug.Log("You are right!");
        }
        else
        {
            if (resultText != null)
                resultText.text = "Wrong answer!";

            Debug.Log("Wrong answer!");
        }
    }
}