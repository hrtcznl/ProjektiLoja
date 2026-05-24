using UnityEngine;

public class RaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera playerCamera;
    public float interactDistance = 3f;

    [Header("UI")]
    public GameObject interactionUI;

    [Header("Toggle Targets")]
    public GameObject linkedObject1;
    public GameObject linkedObject2;

    [Header("Player Control")]
    public FPSController playerController;

    private GameObject lastInteractedObject;
    private bool linkedObjectsEnabled;
    private Renderer[] objectRenderers;
    private Collider[] objectColliders;

    void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>(true);
        objectColliders = GetComponentsInChildren<Collider>(true);
    }

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<FPSController>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        bool hitInteractable = false;
        GameObject hitObject = null;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.GetComponent<RaycastInteraction>() != null)
            {
                hitInteractable = true;
                hitObject = hit.collider.gameObject;

                if (interactionUI != null)
                    interactionUI.SetActive(true);
            }
        }

        if (!hitInteractable && interactionUI != null)
            interactionUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (linkedObjectsEnabled)
            {
                DisableLinkedAndRestorePlayerObject();
            }
            else if (hitInteractable && hitObject != null)
            {
                DisableObjectAndEnableLinked(hitObject);
            }
        }
    }

    private void DisableObjectAndEnableLinked(GameObject target)
    {
        RaycastInteraction targetInteraction = target.GetComponent<RaycastInteraction>();
        if (targetInteraction != null)
            targetInteraction.SetInteractableActive(false);
        else
            target.SetActive(false);

        lastInteractedObject = target;
        linkedObjectsEnabled = true;
        SetLinkedObjectsActive(true);
        SetPlayerControl(false);

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void DisableLinkedAndRestorePlayerObject()
    {
        SetLinkedObjectsActive(false);
        linkedObjectsEnabled = false;
        SetPlayerControl(true);

        if (lastInteractedObject != null)
        {
            // Disable the original object with this script after the second press
            lastInteractedObject.SetActive(false);
        }
    }

    public void SetInteractableActive(bool state)
    {
        foreach (Renderer r in objectRenderers)
            if (r != null)
                r.enabled = state;

        foreach (Collider c in objectColliders)
            if (c != null)
                c.enabled = state;
    }

    private void SetLinkedObjectsActive(bool state)
    {
        if (linkedObject1 != null)
            linkedObject1.SetActive(state);

        if (linkedObject2 != null)
            linkedObject2.SetActive(state);
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerController != null)
            playerController.canMove = enabled;
    }
}