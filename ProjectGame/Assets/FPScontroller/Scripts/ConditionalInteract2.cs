using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ConditionalInteract2 : MonoBehaviour
{
    [Header("Interaction Trigger")]
    public Collider interactionCollider;
    public GameObject triggeringObject;
    public string triggeringTag = "Player";

    [Header("Interaction")]
    public GameObject interactUI;

    [Header("Condition")]
    public GameObject[] dependentObjects;

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

    [Header("Objects To Switch")]
    public GameObject playerObject;
    public GameObject roverObject;

    [Header("Switch UI")]
    public GameObject switchUIImage;

    [Header("Rover Reset")]
    public GameObject timerResetObject;
    public Vector3 resetPosition;
    public Vector3 resetRotationEuler;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public GameObject timerStartTrigger;
    public float timerDuration = 120f;

    private Coroutine uiCoroutine;
    private Coroutine lightCoroutine;
    private Coroutine timerCoroutine;

    private bool interactionLocked = false;
    private bool isPlayerInTrigger = false;
    private Collider[] overlapResults = new Collider[16];
    private Vector3 roverInitialPosition;
    private Quaternion roverInitialRotation;
    private bool timerActive = false;
    private float timeRemaining = 0f;
    private bool timerStartTriggerWasActive = false;

    void Start()
    {
        if (interactionCollider == null)
        {
            interactionCollider = GetComponent<Collider>();
        }

        if (interactionCollider != null && !interactionCollider.isTrigger)
        {
            interactionCollider.isTrigger = true;
        }

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

        if (switchUIImage != null)
            switchUIImage.SetActive(false);

        if (timerText != null)
            timerText.enabled = false;

        if (timerStartTrigger != null)
            timerStartTriggerWasActive = timerStartTrigger.activeSelf;

        if (roverObject != null)
        {
            roverInitialPosition = roverObject.transform.position;
            roverInitialRotation = roverObject.transform.rotation;
        }

        timeRemaining = timerDuration;
    }

    void Update()
    {
        if (interactionLocked)
            return;

        UpdateTriggerState();
        UpdateTimer();
        CheckTimerStartTrigger();

        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void CheckTimerStartTrigger()
    {
        if (timerStartTrigger == null)
            return;

        if (timerStartTrigger.activeSelf && !timerStartTriggerWasActive && !timerActive)
        {
            timerStartTriggerWasActive = true;
            if (timerText != null)
                timerText.enabled = true;
            timerActive = true;
            timeRemaining = timerDuration;
        }
        else if (!timerStartTrigger.activeSelf && timerStartTriggerWasActive)
        {
            timerStartTriggerWasActive = false;
        }
    }

    void UpdateTimer()
    {
        if (!timerActive)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerActive = false;
            if (timerText != null)
            {
                timerText.text = "Rover's battery died! 0:00";
            }
            StartCoroutine(TimerExpiredSequence());
            return;
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes}:{seconds:D2}";
    }

    IEnumerator TimerExpiredSequence()
    {
        if (timerText != null)
        {
            timerText.text = "Rover's battery died!\n 0:00";
        }

        yield return new WaitForSeconds(1.5f);

        interactionLocked = true;

        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);

        lightCoroutine = StartCoroutine(TransitionSequenceInternal(true, false));
    }

    void UpdateTriggerState()
    {
        if (interactionCollider == null)
        {
            isPlayerInTrigger = false;
            return;
        }

        int overlapCount = GetOverlapColliders(overlapResults);
        bool found = false;

        for (int i = 0; i < overlapCount; i++)
        {
            if (IsTriggeringObject(overlapResults[i]))
            {
                found = true;
                break;
            }
        }

        if (found && !isPlayerInTrigger)
        {
            isPlayerInTrigger = true;
            if (interactUI != null)
                interactUI.SetActive(true);
        }
        else if (!found && isPlayerInTrigger)
        {
            isPlayerInTrigger = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    int GetOverlapColliders(Collider[] results)
    {
        if (interactionCollider is BoxCollider box)
        {
            Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
            Vector3 center = box.transform.TransformPoint(box.center);
            return Physics.OverlapBoxNonAlloc(center, halfExtents, results, box.transform.rotation);
        }

        if (interactionCollider is SphereCollider sphere)
        {
            Vector3 center = sphere.transform.TransformPoint(sphere.center);
            float radius = sphere.radius * Mathf.Max(sphere.transform.lossyScale.x, sphere.transform.lossyScale.y, sphere.transform.lossyScale.z);
            return Physics.OverlapSphereNonAlloc(center, radius, results);
        }

        if (interactionCollider is CapsuleCollider capsule)
        {
            Vector3 center = capsule.transform.TransformPoint(capsule.center);
            float radius = capsule.radius * Mathf.Max(capsule.transform.lossyScale.x, capsule.transform.lossyScale.z);
            float height = Mathf.Max(0f, capsule.height * capsule.transform.lossyScale.y * 0.5f - radius);
            Vector3 direction = Vector3.up;

            switch (capsule.direction)
            {
                case 0: direction = Vector3.right; break;
                case 1: direction = Vector3.up; break;
                case 2: direction = Vector3.forward; break;
            }

            Vector3 point0 = center + capsule.transform.rotation * (direction * height);
            Vector3 point1 = center - capsule.transform.rotation * (direction * height);
            return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results);
        }

        if (interactionCollider is MeshCollider mesh && mesh.convex)
        {
            Vector3 center = mesh.bounds.center;
            Vector3 halfExtents = mesh.bounds.extents;
            return Physics.OverlapBoxNonAlloc(center, halfExtents, results, mesh.transform.rotation);
        }

        Bounds bounds = interactionCollider.bounds;
        return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results, Quaternion.identity);
    }

    bool IsTriggeringObject(Collider other)
    {
        if (triggeringObject != null)
        {
            return other.gameObject == triggeringObject || other.transform.IsChildOf(triggeringObject.transform);
        }

        if (!string.IsNullOrEmpty(triggeringTag))
        {
            return other.CompareTag(triggeringTag);
        }

        return true;
    }

    void Interact()
    {
        if (dependentObjects == null || dependentObjects.Length == 0)
            return;

        if (interactUI != null)
            interactUI.SetActive(false);

        bool timerSuccess = timerActive && timeRemaining > 0f;
        bool hasEnoughOil = AreAllDependenciesDisabled();

        if (!hasEnoughOil)
        {
            if (uiObjectToEnable != null)
            {
                uiObjectToEnable.SetActive(true);

                if (uiCoroutine != null)
                    StopCoroutine(uiCoroutine);

                uiCoroutine = StartCoroutine(DisableUIAfterTime());
            }
        }
        else
        {
            if (worldObjectToEnable != null)
                worldObjectToEnable.SetActive(true);

            interactionLocked = true;

            if (lightCoroutine != null)
                StopCoroutine(lightCoroutine);

            lightCoroutine = StartCoroutine(FadeLights(timerSuccess));
        }
    }

    bool AreAllDependenciesDisabled()
    {
        foreach (GameObject obj in dependentObjects)
        {
            if (obj != null && obj.activeSelf)
                return false;
        }
        return true;
    }

    IEnumerator DisableUIAfterTime()
    {
        yield return new WaitForSeconds(uiDisplayTime);

        if (uiObjectToEnable != null)
            uiObjectToEnable.SetActive(false);
    }

    IEnumerator FadeLights(bool doSwitch)
    {
        float time = 0f;

        if (spotLights == null || spotLights.Length == 0)
        {
            StartCoroutine(TransitionSequenceInternal(false, doSwitch));
            yield break;
        }

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
            t = t * t; // Exponential ease-in for gradual start

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

        StartCoroutine(TransitionSequenceInternal(false, doSwitch));
    }

    IEnumerator TransitionSequenceInternal(bool isTimerExpired, bool doSwitch)
    {
        if (fadeImage == null)
            yield break;

        float time = 0f;

        if (worldObjectToEnable != null)
            worldObjectToEnable.SetActive(false);

        while (time < imageFadeTime)
        {
            time += Time.deltaTime;
            SetImageAlpha(time / imageFadeTime);
            yield return null;
        }

        SetImageAlpha(1f);

        if (doSwitch)
        {
            if (playerObject != null)
                playerObject.SetActive(false);

            if (roverObject != null)
                roverObject.SetActive(true);

            if (switchUIImage != null)
                switchUIImage.SetActive(true);
        }

        GameObject objectToReset = timerResetObject != null ? timerResetObject : roverObject;

        if (objectToReset != null)
        {
            if (isTimerExpired)
            {
                objectToReset.transform.position = resetPosition;
                objectToReset.transform.rotation = Quaternion.Euler(resetRotationEuler);

                Rigidbody rb = objectToReset.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (dependentObjects != null)
                {
                    foreach (GameObject obj in dependentObjects)
                    {
                        if (obj != null)
                            obj.SetActive(true);
                    }
                }
            }
            objectToReset.SetActive(true);
        }

        if (isTimerExpired)
        {
            timeRemaining = timerDuration;
            if (timerText != null)
                timerText.text = $"{Mathf.FloorToInt(timerDuration / 60f)}:{Mathf.FloorToInt(timerDuration % 60f):D2}";
        }

        time = 0f;

        while (time < imageFadeTime)
        {
            time += Time.deltaTime;
            SetImageAlpha(1f - (time / imageFadeTime));
            yield return null;
        }

        SetImageAlpha(0f);

        if (isTimerExpired)
        {
            interactionLocked = false;
            timerActive = false;
            timerStartTriggerWasActive = false;
            if (timerText != null)
                timerText.enabled = false;
        }
    }

    void SetImageAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
