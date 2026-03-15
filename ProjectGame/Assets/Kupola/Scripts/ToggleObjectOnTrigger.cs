using UnityEngine;

public class ToggleObjectOnTrigger : MonoBehaviour
{
    public GameObject targetObject;   // Object to disable/enable
    public Collider playerCollider;   // Player collider reference

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            targetObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            targetObject.SetActive(true);
        }
    }
}