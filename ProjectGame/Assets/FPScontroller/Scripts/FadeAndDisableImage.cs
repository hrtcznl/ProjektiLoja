using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeAndDisableImage : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1f;

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

        Color startColor = imageComponent.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            imageComponent.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                alpha
            );

            yield return null;
        }

        // Ensure fully invisible
        imageComponent.color = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            0f
        );

        // Disable the image object
        gameObject.SetActive(false);
    }
}