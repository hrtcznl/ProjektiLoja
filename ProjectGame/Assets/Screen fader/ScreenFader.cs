using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Optional Object To Hide During Fade")]
    public GameObject objectToHide;

    private void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeOut()
    {
        if (objectToHide != null)
            objectToHide.SetActive(false);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1f);

        // Cursor unlocked at end of fade out
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1 - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0f);

        // Optional: lock cursor when gameplay starts
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        gameObject.SetActive(false);
    }
}