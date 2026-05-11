using UnityEngine;

public class ToggleObjectButtons : MonoBehaviour
{
    public GameObject targetObject;

    public void EnableObject()
    {
        if (targetObject == null) return;
        targetObject.SetActive(true);
    }

    public void DisableObject()
    {
        if (targetObject == null) return;
        targetObject.SetActive(false);
    }
}