using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideoButton : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public GameObject videoObject; // Optional

    [Header("Play Settings")]
    public bool enableVideoObject = true;
    public bool restartVideo = true;

    [Header("End Settings")]
    public bool disableWhenFinished = false;
    public GameObject disableOnEndObject1;
    public GameObject disableOnEndObject2;
    public KeyCode skipKey = KeyCode.M;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlayVideo);
        }

        // Hide video object at start
        if (videoObject != null)
        {
            videoObject.SetActive(false);
        }

        // Detect when video finishes
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void Update()
    {
        if (videoPlayer != null && videoPlayer.isPlaying && Input.GetKeyDown(skipKey))
        {
            StopVideo();
        }
    }

    public void PlayVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("No VideoPlayer assigned.");
            return;
        }

        // Enable video object
        if (videoObject != null && enableVideoObject)
        {
            videoObject.SetActive(true);
        }

        // Restart from beginning
        if (restartVideo)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }

        videoPlayer.Play();
    }

    private void StopVideo()
    {
        if (videoPlayer == null)
        {
            return;
        }

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        OnVideoFinished(videoPlayer);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (disableWhenFinished && videoObject != null)
        {
            videoObject.SetActive(false);
        }

        if (disableOnEndObject1 != null)
        {
            disableOnEndObject1.SetActive(false);
        }

        if (disableOnEndObject2 != null)
        {
            disableOnEndObject2.SetActive(false);
        }
    }
}