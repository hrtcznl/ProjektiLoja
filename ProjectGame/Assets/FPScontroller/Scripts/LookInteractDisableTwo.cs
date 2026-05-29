using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LookInteractDisableTwo : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public float holdDuration = 1f;

    [Header("References")]
    public Camera playerCamera;
    public GameObject interactionUI;
    public GameObject holdLabel;
    public Slider holdSlider;

    [Header("Objects to Disable on Interact")]
    public GameObject objectToDisable1;
    public GameObject objectToDisable2;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public GameObject videoObject;
    public bool enableVideoObject = true;
    public bool restartVideo = true;
    public bool disableVideoObjectWhenFinished = true;

    [Header("Fade")]
    public Image fadeImage; // assign a black Image in inspector
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    
    [Header("Restart")]
    public bool restartWholeGameOnVideoEnd = true;
    public string sceneToLoadOnRestart = ""; // leave empty to load build index 0
    private bool isLookingAt;
    private float holdTimer;
    
    [Header("Disable On Video Start")]
    public GameObject disableOnVideoStart1;
    public GameObject disableOnVideoStart2;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }

        if (videoObject != null)
        {
            videoObject.SetActive(false);
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (holdSlider != null)
        {
            holdSlider.minValue = 0f;
            holdSlider.maxValue = 1f;
            holdSlider.value = 0f;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
    }

    void Update()
    {
        CheckLook();

        if (isLookingAt && Input.GetKey(interactKey) && interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (!CanInteract())
        {
            ResetHold();
            return;
        }

        if (isLookingAt && Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;
            float normalizedProgress = holdDuration > 0f ? holdTimer / holdDuration : 1f;

            if (holdSlider != null)
            {
                holdSlider.value = Mathf.Clamp01(normalizedProgress);
            }

            if (holdLabel != null)
            {
                holdLabel.SetActive(true);
            }

            if (holdTimer >= holdDuration)
            {
                Interact();
            }
        }
        else
        {
            ResetHold();
        }
    }

    void CheckLook()
    {
        isLookingAt = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform && CanInteract())
            {
                isLookingAt = true;
            }
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(isLookingAt);
        }
    }

    bool CanInteract()
    {
        return objectToDisable1 != null && objectToDisable1.activeInHierarchy &&
               objectToDisable2 != null && objectToDisable2.activeInHierarchy;
    }

    void ResetHold()
    {
        if (holdTimer == 0f)
        {
            return;
        }

        holdTimer = 0f;

        if (holdSlider != null)
        {
            holdSlider.value = 0f;
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }
    }

    void Interact()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }

        PlayVideo();

        if (objectToDisable1 != null)
        {
            objectToDisable1.SetActive(false);
        }

        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
        }
    }

    void PlayVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("No VideoPlayer assigned.");
            return;
        }

        StartCoroutine(PlayVideoWithFade());
    }

    System.Collections.IEnumerator PlayVideoWithFade()
    {
        if (fadeImage != null && fadeInDuration > 0f)
        {
            yield return StartCoroutine(FadeImage(0f, 1f, fadeInDuration));
        }

        if (videoObject != null && enableVideoObject)
        {
            videoObject.SetActive(true);
        }

        // disable optional objects when the video starts
        if (disableOnVideoStart1 != null)
        {
            disableOnVideoStart1.SetActive(false);
        }

        if (disableOnVideoStart2 != null)
        {
            disableOnVideoStart2.SetActive(false);
        }

        if (restartVideo)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }

        videoPlayer.Play();

        if (fadeImage != null && fadeOutDuration > 0f)
        {
            yield return StartCoroutine(FadeImage(1f, 0f, fadeOutDuration));
        }
    }

    System.Collections.IEnumerator FadeImage(float from, float to, float duration)
    {
        if (fadeImage == null)
            yield break;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = duration > 0f ? Mathf.Lerp(from, to, t / duration) : to;
            c.a = a;
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (disableVideoObjectWhenFinished && videoObject != null)
        {
            videoObject.SetActive(false);
        }
        
        if (restartWholeGameOnVideoEnd)
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // ensure time scale is normal
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(sceneToLoadOnRestart))
        {
            SceneManager.LoadScene(sceneToLoadOnRestart);
        }
        else
        {
            // load first scene in build settings (index 0)
            SceneManager.LoadScene(0);
        }
    }
}