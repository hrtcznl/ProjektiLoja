using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFadeListener : MonoBehaviour
{
    [Header("References")]
    public MainMenuController menu;
    public Image img;

    [Header("Fade Settings")]
    public float fadeTime = 1f;

    private bool isFading = false;
    private Button playButton;

    void Awake()
    {
        if (img == null)
            img = GetComponent<Image>();

        SetAlpha(1f);
    }

    void Start()
    {
        if (menu == null)
            return;

        playButton = menu.GetComponentInChildren<Button>();

        if (playButton != null)
            playButton.onClick.AddListener(StartFade);
    }

    void StartFade()
    {
        if (isFading)
            return;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        isFading = true;

        float t = 0f;

        float startAlpha = 1f;
        float endAlpha = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            float a = Mathf.Lerp(startAlpha, endAlpha, t / fadeTime);
            SetAlpha(a);

            yield return null;
        }

        SetAlpha(0f);
        isFading = false;
    }

    void SetAlpha(float a)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}