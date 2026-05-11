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

    [Header("Indicators")]
    public GameObject rightIndicator;
    public GameObject wrongIndicator;
    public GameObject alreadySolvedIndicator;
    public float indicatorDisplayTime = 2f;

    private Coroutine indicatorCoroutine;
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
        HideAllIndicators();
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
            ShowIndicator(alreadySolvedIndicator);
            return;
        }

        if (pressedButtonID == currentCorrectButtonID)
        {
            Debug.Log("You are right!");
            puzzleSolved = true;
            ShowIndicator(rightIndicator);
        }
        else
        {
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

    System.Collections.IEnumerator HideIndicatorAfterDelay(GameObject indicator)
    {
        yield return new WaitForSeconds(indicatorDisplayTime);

        if (indicator != null)
            indicator.SetActive(false);
    }
}