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

    private int triggerActivationCount = 0;

    private void Start()
    {
        // Default state:
        // First array enabled
        // Second array disabled

        SetObjects(disableObjects, true);
        SetObjects(enableObjects, false);
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
}