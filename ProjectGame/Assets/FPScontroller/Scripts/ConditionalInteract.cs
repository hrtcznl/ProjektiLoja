using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConditionalInteract : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public GameObject interactUI;

    [Header("Condition")]
    public GameObject dependentObject;
    public GameObject requiredObject;

    [Header("Results")]
    public GameObject uiObjectToEnable;     
    public GameObject worldObjectToEnable;

    [Header("UI Timing")]
    public float uiDisplayTime = 3f;

    [Header("Lights")]
    public Light[] spotLights;
    public float targetIntensity = 900f;
    public float lightFadeTime = 2f;

    [Header("Transition Image")]
    public Image fadeImage;
    public float imageFadeTime = 1f;

    [Header("Restore")]
    public float restoreDelay = 10f;
    public GameObject restoreObjectToEnable;
    public GameObject alternativeUIObject;

    [Header("Objects To Switch")]
    public GameObject playerObject;
    public GameObject roverObject;

    private Coroutine uiCoroutine;
    private Coroutine lightCoroutine;
    private Coroutine restoreCoroutine;

    private bool interactionLocked = false;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (spotLights != null)
        {
            foreach (Light l in spotLights)
            {
                if (l != null)
                    l.intensity = 0f;
            }
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        HandleLookAndInteract();
    }

    void HandleLookAndInteract()
    {
        if (interactionLocked)
        {
            if (interactUI != null)
                interactUI.SetActive(false);
            return;
        }

        bool looking = false;

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.transform.root.gameObject == gameObject)
            {
                if (requiredObject == null || requiredObject.activeSelf)
                {
                    looking = true;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Interact();
                    }
                }
            }
        }

        if (interactUI != null)
            interactUI.SetActive(looking);
    }

    void Interact()
    {
        if (requiredObject != null && !requiredObject.activeSelf)
            return;

        if (restoreObjectToEnable != null && restoreObjectToEnable.activeSelf)
        {
            if (alternativeUIObject != null)
            {
                alternativeUIObject.SetActive(true);

                if (uiCoroutine != null)
                    StopCoroutine(uiCoroutine);

                uiCoroutine = StartCoroutine(DisableUIAfterTime(alternativeUIObject));
            }
            return;
        }

        if (dependentObject == null)
            return;

        if (!dependentObject.activeSelf)
        {
            if (uiObjectToEnable != null)
            {
                uiObjectToEnable.SetActive(true);

                if (uiCoroutine != null)
                    StopCoroutine(uiCoroutine);

                uiCoroutine = StartCoroutine(DisableUIAfterTime(uiObjectToEnable));
            }
        }
        else
        {
            if (worldObjectToEnable != null)
                worldObjectToEnable.SetActive(true);

            dependentObject.SetActive(false);

            if (requiredObject != null)
                requiredObject.SetActive(false);

            interactionLocked = true;

            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            if (restoreCoroutine != null)
                StopCoroutine(restoreCoroutine);

            lightCoroutine = StartCoroutine(FadeLights());
            restoreCoroutine = StartCoroutine(RestoreResultsAfterDelay());
        }
    }

    IEnumerator DisableUIAfterTime(GameObject uiToDisable)
    {
        yield return new WaitForSeconds(uiDisplayTime);

        if (uiToDisable != null)
            uiToDisable.SetActive(false);
    }

    IEnumerator FadeLights()
    {
        float time = 0f;

        float[] startIntensities = new float[spotLights.Length];

        for (int i = 0; i < spotLights.Length; i++)
        {
            if (spotLights[i] != null)
                startIntensities[i] = spotLights[i].intensity;
        }

        while (time < lightFadeTime)
        {
            time += Time.deltaTime;
            float t = time / lightFadeTime;

            for (int i = 0; i < spotLights.Length; i++)
            {
                if (spotLights[i] != null)
                {
                    spotLights[i].intensity = Mathf.Lerp(
                        startIntensities[i],
                        targetIntensity,
                        t
                    );
                }
            }

            yield return null;
        }

        for (int i = 0; i < spotLights.Length; i++)
        {
            if (spotLights[i] != null)
                spotLights[i].intensity = targetIntensity;
        }

        StartCoroutine(TransitionSequence());
    }

    IEnumerator FadeLightsToZero()
    {
        float time = 0f;

        float[] startIntensities = new float[spotLights.Length];

        for (int i = 0; i < spotLights.Length; i++)
        {
            if (spotLights[i] != null)
                startIntensities[i] = spotLights[i].intensity;
        }

        while (time < lightFadeTime)
        {
            time += Time.deltaTime;
            float t = time / lightFadeTime;

            for (int i = 0; i < spotLights.Length; i++)
            {
                if (spotLights[i] != null)
                {
                    spotLights[i].intensity = Mathf.Lerp(
                        startIntensities[i],
                        0f,
                        t
                    );
                }
            }

            yield return null;
        }

        for (int i = 0; i < spotLights.Length; i++)
        {
            if (spotLights[i] != null)
                spotLights[i].intensity = 0f;
        }
    }

    IEnumerator RestoreResultsAfterDelay()
    {
        yield return new WaitForSeconds(restoreDelay);

        if (restoreObjectToEnable != null)
            restoreObjectToEnable.SetActive(true);

        interactionLocked = false;

        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);

        lightCoroutine = StartCoroutine(FadeLightsToZero());
    }

    IEnumerator TransitionSequence()
    {
        if (fadeImage == null)
            yield break;

        float time = 0f;

        while (time < imageFadeTime)
        {
            time += Time.deltaTime;
            SetImageAlpha(time / imageFadeTime);
            yield return null;
        }

        SetImageAlpha(1f);

        if (playerObject != null)
            playerObject.SetActive(false);

        if (roverObject != null)
            roverObject.SetActive(true);

        time = 0f;

        while (time < imageFadeTime)
        {
            time += Time.deltaTime;
            SetImageAlpha(1f - (time / imageFadeTime));
            yield return null;
        }

        SetImageAlpha(0f);
    }

    void SetImageAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
