using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pauseObject;
    public MonoBehaviour playerMovement;

    private bool isPaused = false;

    void Start()
    {
        pauseObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        ApplyState();
    }

    void ApplyState()
    {
        pauseObject.SetActive(isPaused);

        if (playerMovement != null)
            playerMovement.enabled = !isPaused;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;   // optional
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;   // optional
        }
    }

    // THIS is what your Resume button will call
    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;
        ApplyState();
    }
}