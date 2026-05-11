using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuObject;
    public Behaviour playerMovementScript;
    public Button playButton;

    private bool isStarting = false;

    void Start()
    {
        mainMenuObject.SetActive(true);
        playerMovementScript.enabled = false;

        playButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        if (isStarting)
            return;

        isStarting = true;

        mainMenuObject.SetActive(false);
        playerMovementScript.enabled = true;
    }
}