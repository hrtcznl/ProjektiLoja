using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsUI;

    public MonoBehaviour playerMovement;
    public MonoBehaviour mouseLook;

    private bool escLocked = false;

    void Start()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        escLocked = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsUI != null)
            optionsUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (escLocked)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (optionsUI != null)
            optionsUI.SetActive(false);

        Time.timeScale = 0f;
        GameIsPaused = true;
        escLocked = true;

        DisablePlayerControls();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsUI != null)
            optionsUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        escLocked = false;

        EnablePlayerControls();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenOptions()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsUI != null)
            optionsUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        DisablePlayerControls();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseOptions()
    {
        if (optionsUI != null)
            optionsUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        DisablePlayerControls();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        escLocked = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("StartMenu");
    }

    void DisablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (mouseLook != null)
            mouseLook.enabled = false;
    }

    void EnablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (mouseLook != null)
            mouseLook.enabled = true;
    }
}