using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject optionsUI;

    void Start()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        // Reset only once per editor play session
        if (!SessionState.GetBool("CutsceneResetThisSession", false))
        {
            PlayerPrefs.DeleteKey("HasSeenIntroCutscene");
            PlayerPrefs.Save();
            SessionState.SetBool("CutsceneResetThisSession", true);
        }
#endif

        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        if (optionsUI != null)
            optionsUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;

        int hasSeenCutscene = PlayerPrefs.GetInt("HasSeenIntroCutscene", 0);

        if (hasSeenCutscene == 0)
        {
            SceneManager.LoadScene("CutsceneScene");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void OpenOptions()
    {
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (optionsUI != null)
            optionsUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        if (optionsUI != null)
            optionsUI.SetActive(false);

        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}