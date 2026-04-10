using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private bool hasLoadedScene = false;

    void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned!");
            return;
        }

        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    void Update()
    {
        if (hasLoadedScene || videoPlayer == null)
            return;

        if (videoPlayer.isPlaying && videoPlayer.time >= videoPlayer.length - 0.1f)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (hasLoadedScene)
            return;

        hasLoadedScene = true;

        PlayerPrefs.SetInt("HasSeenIntroCutscene", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }
}