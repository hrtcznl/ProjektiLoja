using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoStarter : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Header("Loading Objects (3)")]
    public GameObject[] loadingObjects;

    public float imageSwitchInterval = 0.3f;
    public float minimumCoverTime = 1.0f;

    [Header("Fade")]
    public ScreenFader screenFader;
    public float fadeStartOffset = 0.5f;

    private bool videoReady = false;
    private float startTime;
    private bool fadeStarted = false;

    private bool cursorHiddenOnce = false;

    void Start()
    {
        startTime = Time.time;

        SetLoadingObject(0);

        // Ensure cursor starts visible (optional safety)
        Cursor.visible = true;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        StartCoroutine(WaitAndPlay());
        StartCoroutine(CycleLoadingObjects());
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoReady = true;
    }

    IEnumerator WaitAndPlay()
    {
        while (true)
        {
            float elapsed = Time.time - startTime;
            bool ready = elapsed >= minimumCoverTime && videoReady;

            if (!fadeStarted && videoReady && elapsed >= minimumCoverTime - fadeStartOffset)
            {
                fadeStarted = true;

                if (screenFader != null)
                    StartCoroutine(screenFader.FadeOut());
            }

            if (ready)
                break;

            yield return null;
        }

        SetLoadingObject(-1);

        videoPlayer.Play();

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeIn());
    }

    IEnumerator CycleLoadingObjects()
    {
        if (loadingObjects == null || loadingObjects.Length == 0)
            yield break;

        int index = 0;

        while (!videoReady || Time.time - startTime < minimumCoverTime)
        {
            SetLoadingObject(index);

            // Hide cursor when second image appears first time
            if (!cursorHiddenOnce && index == 1)
            {
                Cursor.visible = false;
                cursorHiddenOnce = true;
            }

            index = (index + 1) % loadingObjects.Length;
            yield return new WaitForSeconds(imageSwitchInterval);
        }

        SetLoadingObject(-1);
    }

    void SetLoadingObject(int activeIndex)
    {
        for (int i = 0; i < loadingObjects.Length; i++)
        {
            if (loadingObjects[i] != null)
                loadingObjects[i].SetActive(i == activeIndex);
        }
    }
}