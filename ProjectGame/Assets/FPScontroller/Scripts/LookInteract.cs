using UnityEngine;

public class LookInteract : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("References")]
    public Camera playerCamera;
    public GameObject interactionUI;
    public GameObject objectToEnable;
    public GameObject dependableObject;

    private bool isLookingAt;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        CheckLook();

        if (isLookingAt && Input.GetKeyDown(interactKey) && IsDependencyMet())
        {
            Interact();
        }
    }

    void CheckLook()
    {
        isLookingAt = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform)
            {
                isLookingAt = true;
            }
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(isLookingAt && IsDependencyMet());
        }
    }

    bool IsDependencyMet()
    {
        if (dependableObject == null)
        {
            return true;
        }

        return dependableObject.activeInHierarchy;
    }

    void Interact()
    {
        // Enable target object
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        // Hide interaction UI
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        // Disable this object
        gameObject.SetActive(false);
    }
}