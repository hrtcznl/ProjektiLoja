using System.Collections;
using UnityEngine;

public class TriggerObjectSwitcher : MonoBehaviour
{
    [Header("Player")]
    public string playerTag = "Player";

    [Header("Objects To Disable When Player Enters")]
    public GameObject[] disableObjects;

    [Header("Objects To Enable When Player Enters")]
    public GameObject[] enableObjects;

    [Header("Additional Trigger Colliders")]
    [SerializeField] private Collider[] additionalTriggerColliders;

    [Header("First-Time Enable Objects")]
    [Tooltip("Objects enabled only the first time the player enters. Will be disabled after the duration.")]
    public GameObject[] firstTimeEnableObjects;

    [Tooltip("Duration (seconds) that the first-time objects stay enabled before being disabled")]
    public float firstTimeEnableDuration = 5f;

    private bool firstTimeTriggered = false;
    private Coroutine firstTimeCoroutine = null;

    private int triggerActivationCount = 0;

    private void Start()
    {
        // Default state:
        // First array enabled
        // Second array disabled

        SetObjects(disableObjects, true);
        SetObjects(enableObjects, false);
        // Ensure first-time objects start disabled
        SetObjects(firstTimeEnableObjects, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        OnPlayerEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        OnPlayerExited();
    }

    /// <summary>
    /// Call this method when the player enters any trigger zone.
    /// Uses reference counting to handle multiple overlapping triggers.
    /// </summary>
    public void OnPlayerEntered()
    {
        triggerActivationCount++;
        
        if (triggerActivationCount == 1)
        {
            SetObjects(disableObjects, false);
            SetObjects(enableObjects, true);
            // If this is the first-ever entry, enable the first-time objects and start timer
            if (!firstTimeTriggered && firstTimeEnableObjects != null && firstTimeEnableObjects.Length > 0)
            {
                firstTimeTriggered = true;
                SetObjects(firstTimeEnableObjects, true);
                if (firstTimeCoroutine != null)
                    StopCoroutine(firstTimeCoroutine);
                firstTimeCoroutine = StartCoroutine(DisableFirstTimeObjectsAfterDelay());
            }
        }
    }

    /// <summary>
    /// Call this method when the player exits any trigger zone.
    /// Uses reference counting to handle multiple overlapping triggers.
    /// </summary>
    public void OnPlayerExited()
    {
        triggerActivationCount--;
        
        if (triggerActivationCount <= 0)
        {
            triggerActivationCount = 0;
            SetObjects(disableObjects, true);
            SetObjects(enableObjects, false);
        }
    }

    private void SetObjects(GameObject[] objects, bool state)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }

    private IEnumerator DisableFirstTimeObjectsAfterDelay()
    {
        yield return new WaitForSeconds(firstTimeEnableDuration);
        SetObjects(firstTimeEnableObjects, false);
        firstTimeCoroutine = null;
    }
}