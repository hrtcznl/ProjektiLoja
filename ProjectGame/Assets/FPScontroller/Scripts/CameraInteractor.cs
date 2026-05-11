using UnityEngine;

public class CameraInteractor : MonoBehaviour
{
    [System.Serializable]
    public class InteractableEntry
    {
        public GameObject worldObject; // object in scene
        public GameObject hudObject;   // HUD symbol to enable
    }

    [Header("Camera")]
    public Camera cam;

    [Header("Interaction")]
    public float interactDistance = 3f;

    [Header("UI")]
    public GameObject interactUI; // "Press E"

    [Header("Objects")]
    public InteractableEntry[] interactables;

    private GameObject currentTarget;
    private InteractableEntry currentEntry;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        HandleLook();
        HandleInteract();
    }

    void HandleLook()
    {
        currentTarget = null;
        currentEntry = null;

        bool looking = false;

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            GameObject hitObj = hit.collider.transform.root.gameObject;

            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].worldObject == hitObj)
                {
                    looking = true;
                    currentTarget = hitObj;
                    currentEntry = interactables[i];
                    break;
                }
            }
        }

        if (interactUI != null)
            interactUI.SetActive(looking);
    }

    void HandleInteract()
    {
        if (currentEntry == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Enable HUD symbol
            if (currentEntry.hudObject != null)
                currentEntry.hudObject.SetActive(true);

            // Disable object
            if (currentEntry.worldObject != null)
                currentEntry.worldObject.SetActive(false);

            // Reset
            currentTarget = null;
            currentEntry = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}