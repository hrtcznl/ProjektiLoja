using UnityEngine;
using UnityEngine.UI;

public class ToggleObject : MonoBehaviour
{
    public Toggle toggle;
    public GameObject targetObject;

    private bool isInitializing = false;

    void Start()
    {
        if (toggle == null || targetObject == null)
            return;

        isInitializing = true;

        // Sync toggle from object state (or object from toggle if you prefer)
        toggle.isOn = targetObject.activeSelf;

        // Ensure object matches toggle
        targetObject.SetActive(toggle.isOn);

        isInitializing = false;

        // Listen for changes
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isInitializing) return;

        if (targetObject != null)
            targetObject.SetActive(isOn);
    }
}