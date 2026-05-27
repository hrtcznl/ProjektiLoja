using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeAndDisableImage : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 3f;
    [Tooltip("Starting delay before the first fade begins.")]
    public float startDelay = 0f;
    [Tooltip("Target alpha for the first fade, specified as 0-255. Use 100 for the alpha value 100.")]
    public int firstFadeTargetAlpha255 = 100;
    [Tooltip("Time to wait after the first fade reaches the target alpha before fading back to full.")]
    public float holdAtTargetAlpha = 0f;
    [Tooltip("Duration to fade from the first fade target alpha back to full.")]
    public float fadeInDuration = 1f;
    [Tooltip("Time to wait after alpha reaches 1 before fading back to 0.")]
    public float holdAtOne = 0f;
    [Tooltip("Duration to fade from 1 back to 0 after hold.")]
    public float fadeOutAfterHoldDuration = 1f;
    [Tooltip("If true, disable the GameObject after the final fade to 0.")]
    public bool disableAfterCycle = true;

    private Image imageComponent;
    private bool isFading = false;

    void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    // Assign this to your UI Button OnClick()
    public void FadeOutAndDisable()
    {
        if (!isFading)
        {
            StartCoroutine(FadeCoroutine());
        }
    }

    IEnumerator FadeCoroutine()
    {
        isFading = true;

        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        Color baseColor = imageComponent.color;

        // Phase 1: fade from current alpha to the target alpha (100/255 by default)
        float startAlpha = baseColor.a;
        float targetAlpha = Mathf.Clamp01(firstFadeTargetAlpha255 / 255f);
        if (fadeDuration > 0f)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / fadeDuration);
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                yield return null;
            }
        }
        // Ensure the target alpha is reached
        imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);

        // Hold at target alpha
        if (holdAtTargetAlpha > 0f)
            yield return new WaitForSeconds(holdAtTargetAlpha);

        // Phase 2: fade from target alpha to 1
        if (fadeInDuration > 0f)
        {
            float timer2 = 0f;
            while (timer2 < fadeInDuration)
            {
                timer2 += Time.deltaTime;
                float t = Mathf.Clamp01(timer2 / fadeInDuration);
                float alpha = Mathf.Lerp(targetAlpha, 1f, t);
                imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                yield return null;
            }
        }
        // Ensure fully visible
        imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        // Hold at one
        if (holdAtOne > 0f)
            yield return new WaitForSeconds(holdAtOne);

        // Phase 3: fade from 1 back to 0
        if (fadeOutAfterHoldDuration > 0f)
        {
            float timer3 = 0f;
            while (timer3 < fadeOutAfterHoldDuration)
            {
                timer3 += Time.deltaTime;
                float t = Mathf.Clamp01(timer3 / fadeOutAfterHoldDuration);
                float alpha = Mathf.Lerp(1f, 0f, t);
                imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                yield return null;
            }
        }
        // Ensure fully invisible
        imageComponent.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        if (disableAfterCycle)
            gameObject.SetActive(false);

        isFading = false;
    }
}