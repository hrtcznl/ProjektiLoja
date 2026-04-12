using UnityEngine;

public class InverseObjectToggle : MonoBehaviour
{
    public GameObject targetObject;

    void OnEnable()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }
}