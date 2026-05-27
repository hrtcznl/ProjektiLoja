using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class PulsingLight : MonoBehaviour
{
    [Header("Intensity Settings")]
    public float minIntensity = 0f;
    public float maxIntensity = 5f;

    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;

    [Header("Audio")]
    public bool playAudioOnMax = true;
    public float peakThreshold = 0.05f;

    [Header("Muffle")]
    public bool useMuffleStartup = true;
    public float initialLowPassCutoff = 500f;
    public float finalLowPassCutoff = 22000f;
    public float muffleClearDuration = 10f;

    [Header("Optional")]
    public bool playOnStart = true;

    private Light lightSource;
    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private bool isPulsing = false;
    private bool hasPlayedOnPeak = false;
    private bool muffleStarted = false;
    private float muffleElapsed = 0f;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();

        if (useMuffleStartup)
        {
            lowPassFilter = GetComponent<AudioLowPassFilter>();
            if (lowPassFilter == null)
            {
                lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
            }

            if (lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = initialLowPassCutoff;
            }
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            StartPulse();
        }
    }

    private void Update()
    {
        if (!isPulsing)
            return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        lightSource.intensity = intensity;

        if (muffleStarted && useMuffleStartup && muffleElapsed < muffleClearDuration)
        {
            muffleElapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(muffleElapsed / muffleClearDuration);
            if (lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = Mathf.Lerp(initialLowPassCutoff, finalLowPassCutoff, ratio);
            }
        }

        if (playAudioOnMax && audioSource != null)
        {
            if (intensity >= maxIntensity - peakThreshold && !hasPlayedOnPeak)
            {
                audioSource.Play();
                hasPlayedOnPeak = true;

                if (useMuffleStartup && !muffleStarted)
                {
                    muffleStarted = true;
                    muffleElapsed = 0f;
                    if (lowPassFilter != null)
                    {
                        lowPassFilter.cutoffFrequency = initialLowPassCutoff;
                    }
                }
            }
            else if (intensity < maxIntensity - peakThreshold)
            {
                hasPlayedOnPeak = false;
            }
        }
    }

    public void StartPulse()
    {
        isPulsing = true;
    }

    public void StopPulse()
    {
        isPulsing = false;
    }
}