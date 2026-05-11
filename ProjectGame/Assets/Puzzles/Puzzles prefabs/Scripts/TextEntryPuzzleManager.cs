using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TextEntryPuzzleOption
{
    public Texture2D image;
    public string correctAnswer;
}

public class TextEntryPuzzleManager : MonoBehaviour
{
    [Header("Physical image screen")]
    public Renderer displayRenderer;

    [Header("Puzzle data")]
    public TextEntryPuzzleOption[] puzzleOptions;

    [Header("Physical text panel")]
    public TMP_Text inputDisplayText;
    public TMP_Text resultText;
    public TMP_Text promptText;

    [Header("Indicators")]
    public GameObject rightIndicator;
    public GameObject wrongIndicator;
    public GameObject alreadySolvedIndicator;
    public float indicatorDisplayTime = 2f;

    private Coroutine indicatorCoroutine;
    private string currentCorrectAnswer = "";
    private string currentInput = "";
    private bool puzzleSolved = false;
    private bool isTyping = false;

    void Start()
    {
        if (displayRenderer == null)
        {
            Debug.LogError("Display Renderer not assigned.");
            return;
        }

        if (puzzleOptions == null || puzzleOptions.Length == 0)
        {
            Debug.LogError("No puzzle options assigned.");
            return;
        }

        if (inputDisplayText == null)
        {
            Debug.LogError("Input Display Text not assigned.");
            return;
        }

        ShowRandomPuzzle();
        HideAllIndicators();
        ShowPromptOnly();
    }

    void Update()
    {
        if (!isTyping || puzzleSolved)
            return;

        HandleTyping();

        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateInputDisplay();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopTyping();
        }
    }

    void HandleTyping()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b' || c == '\n' || c == '\r')
                continue;

            if (char.IsControl(c))
                continue;

            if (!(char.IsDigit(c) || c == '.' || c == 'k' || c == 'K' || c == 'm' || c == 'M'))
                continue;

            currentInput += c;
            UpdateInputDisplay();
        }
    }

    void UpdateInputDisplay()
    {
        if (inputDisplayText != null)
            inputDisplayText.text = currentInput;
    }

    void ShowRandomPuzzle()
    {
        int index = Random.Range(0, puzzleOptions.Length);
        TextEntryPuzzleOption selectedPuzzle = puzzleOptions[index];

        if (selectedPuzzle.image == null)
        {
            Debug.LogError("Puzzle image is missing.");
            return;
        }

        Material mat = displayRenderer.material;

        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", selectedPuzzle.image);

        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", selectedPuzzle.image);

        currentCorrectAnswer = CleanText(selectedPuzzle.correctAnswer);
        currentInput = "";
        puzzleSolved = false;
        isTyping = false;

        UpdateInputDisplay();
    }

    public void StartTyping()
    {
        if (puzzleSolved)
        {
            ShowResultOnly("Puzzle solved");
            ShowIndicator(alreadySolvedIndicator);
            return;
        }

        isTyping = true;
        currentInput = "";
        UpdateInputDisplay();
        ShowInputOnly();
    }

    public void StopTyping()
    {
        isTyping = false;

        if (!puzzleSolved)
        {
            ShowPromptOnly();
        }
    }

    public void SubmitAnswer()
    {
        if (puzzleSolved)
        {
            ShowResultOnly("Puzzle already solved.");
            return;
        }

        string playerAnswer = CleanText(currentInput);
        isTyping = false;

        if (playerAnswer == currentCorrectAnswer)
        {
            puzzleSolved = true;
            ShowResultOnly("You are right!");
            Debug.Log("You are right!");
            ShowIndicator(rightIndicator);
        }
        else
        {
            ShowResultOnly("Wrong answer!");
            Debug.Log("Wrong answer!");
            ShowIndicator(wrongIndicator);
        }
    }

    void HideAllIndicators()
    {
        if (rightIndicator != null)
            rightIndicator.SetActive(false);

        if (wrongIndicator != null)
            wrongIndicator.SetActive(false);

        if (alreadySolvedIndicator != null)
            alreadySolvedIndicator.SetActive(false);
    }

    void ShowIndicator(GameObject indicator)
    {
        HideAllIndicators();

        if (indicator == null)
            return;

        indicator.SetActive(true);

        if (indicatorCoroutine != null)
            StopCoroutine(indicatorCoroutine);

        indicatorCoroutine = StartCoroutine(HideIndicatorAfterDelay(indicator));
    }

    IEnumerator HideIndicatorAfterDelay(GameObject indicator)
    {
        yield return new WaitForSeconds(indicatorDisplayTime);

        if (indicator != null)
            indicator.SetActive(false);
    }

    string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        text = text.Trim().ToLower();

        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }

        return text;
    }

    void ShowPromptOnly()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(true);

        if (inputDisplayText != null)
            inputDisplayText.gameObject.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void ShowInputOnly()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (inputDisplayText != null)
        {
            inputDisplayText.gameObject.SetActive(true);
            inputDisplayText.text = currentInput;
        }

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void ShowResultOnly(string message)
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (inputDisplayText != null)
            inputDisplayText.gameObject.SetActive(false);

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = message;
        }
    }
}