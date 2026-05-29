using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class VideoFinishAnimator : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public Animator targetAnimator;
    public FadeAndDisableImage fadeAndDisableImage;
    public GameObject objectToEnable;
    public GameObject secondObjectToEnable;

    [Header("Timing")]
    public float delayAfterVideoEnds = 1f;

    [Header("Instant Finish Key")]
    public KeyCode skipKey = KeyCode.M;

    [Header("Optional")]
    public bool stopVideoOnSkip = true;

    private bool triggered = false;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        if (targetAnimator != null)
        {
            targetAnimator.enabled = false;
        }
    }

    void Update()
    {
        if (triggered)
            return;

        if (Input.GetKeyDown(skipKey))
        {
            FinishVideoInstantly();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (triggered)
            return;

        triggered = true;
        EnableAnimatorAfterDelayAsync();
    }

    void FinishVideoInstantly()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.canSetTime)
            {
                videoPlayer.time = videoPlayer.length;
            }

            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }

        triggered = true;
        EnableAnimatorAfterDelayAsync();
    }

    async void EnableAnimatorAfterDelayAsync()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(delayAfterVideoEnds));

        if (this == null)
            return;

        if (fadeAndDisableImage != null)
        {
            fadeAndDisableImage.FadeOutAndDisable();
        }

        if (targetAnimator != null)
        {
            targetAnimator.enabled = true;
        }

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        if (secondObjectToEnable != null)
        {
            secondObjectToEnable.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}