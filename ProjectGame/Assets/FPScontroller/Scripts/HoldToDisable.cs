using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HoldToDisable : MonoBehaviour
{
    [Header("Objects that can be disabled")]
    public GameObject[] targetObjects;

    [Header("Hold Settings")]
    public float holdTime = 2f;

    [Header("UI Settings")]
    public GameObject uiObject;
    public Slider progressSlider;
    public TextMeshProUGUI counterText;

    private GameObject currentTarget;
    private float holdTimer;
    private bool playerInRange;

    private void Start()
    {
        UpdateCounterText();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject obj in targetObjects)
        {
            if (other.gameObject == obj)
            {
                currentTarget = obj;
                playerInRange = true;
                holdTimer = 0f;
                if (progressSlider != null)
                {
                    progressSlider.value = progressSlider.minValue;
                }
                if (uiObject != null)
                {
                    uiObject.SetActive(true);
                }
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentTarget)
        {
            playerInRange = false;
            currentTarget = null;
            holdTimer = 0f;
            if (uiObject != null)
            {
                uiObject.SetActive(false);
            }
            if (progressSlider != null)
            {
                progressSlider.value = progressSlider.minValue;
            }
        }
    }

    private void Update()
    {
        if ((currentTarget == null || !currentTarget.activeInHierarchy) && uiObject != null && uiObject.activeSelf)
        {
            uiObject.SetActive(false);
            if (progressSlider != null)
            {
                progressSlider.value = progressSlider.minValue;
            }
        }

        UpdateCounterText();

        if (playerInRange && currentTarget != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime;
                if (progressSlider != null)
                {
                    progressSlider.value = Mathf.Lerp(progressSlider.minValue, progressSlider.maxValue, Mathf.Clamp01(holdTimer / holdTime));
                }

                if (holdTimer >= holdTime)
                {
                    currentTarget.SetActive(false);
                    UpdateCounterText();

                    if (uiObject != null)
                    {
                        uiObject.SetActive(false);
                    }
                    if (progressSlider != null)
                    {
                        progressSlider.value = progressSlider.minValue;
                    }

                    holdTimer = 0f;
                    playerInRange = false;
                    currentTarget = null;
                }
            }
            else
            {
                holdTimer = 0f;
                if (progressSlider != null)
                {
                    progressSlider.value = progressSlider.minValue;
                }
            }
        }
    }

    private void UpdateCounterText()
    {
        if (counterText != null)
        {
            int totalCount = targetObjects != null ? targetObjects.Length : 0;
            int disabledCount = 0;
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null && !obj.activeInHierarchy)
                    disabledCount++;
            }
            counterText.text = disabledCount + "/" + totalCount + " L";
        }
    }
}