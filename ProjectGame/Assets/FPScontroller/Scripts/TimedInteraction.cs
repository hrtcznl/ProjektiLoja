using UnityEngine;
using UnityEngine.UI;

public class TimedInteraction : MonoBehaviour
{
    [Header("Player")]
    public Camera playerCamera;
    public float interactionDistance = 3f;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionTime = 5f;

    [Tooltip("This object must be enabled for interaction to work")]
    public GameObject requiredObject;

    [Tooltip("Object to enable after the interaction completes")]
    public GameObject objectToEnable;

    [Header("UI")]
    public GameObject interactionIndicator;
    public GameObject interactionUI;
    public Slider progressSlider;

    private bool isLookingAtObject = false;
    private bool isInteracting = false;
    private float currentInteractionTime = 0f;

    void Start()
    {
        // Hide interaction UI at start
        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0f;
        }
    }

    void Update()
    {
        // If required object is missing or disabled, stop everything
        if (requiredObject == null || !requiredObject.activeInHierarchy)
        {
            HideAllUI();
            return;
        }

        CheckLookAtObject();

        // Show interaction prompt only if not already interacting
        if (isLookingAtObject && !isInteracting)
        {
            if (interactionIndicator != null)
                interactionIndicator.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                StartInteraction();
            }
        }
        else
        {
            if (!isInteracting && interactionIndicator != null)
                interactionIndicator.SetActive(false);
        }

        // Handle timed interaction
        if (isInteracting)
        {
            if (Input.GetKey(interactKey))
            {
                currentInteractionTime += Time.deltaTime;

                // Update slider
                if (progressSlider != null)
                {
                    progressSlider.value = currentInteractionTime / interactionTime;
                }

                // Finish interaction
                if (currentInteractionTime >= interactionTime)
                {
                    CompleteInteraction();
                }
            }
            else
            {
                CancelInteraction();
            }
        }
    }

    void CheckLookAtObject()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        isLookingAtObject = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isLookingAtObject = true;
            }
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        currentInteractionTime = 0f;

        // Hide interaction indicator
        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        // Enable interaction UI
        if (interactionUI != null)
            interactionUI.SetActive(true);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = 0f;
        }
    }

    void CancelInteraction()
    {
        isInteracting = false;
        currentInteractionTime = 0f;

        // Hide interaction UI
        if (interactionUI != null)
            interactionUI.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }
    }

    void CompleteInteraction()
    {
        // Disable UI
        if (interactionUI != null)
            interactionUI.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }

        // Swap object states
        if (requiredObject != null)
            requiredObject.SetActive(false);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        // Disable this script
        enabled = false;
    }

    void HideAllUI()
    {
        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        if (interactionUI != null)
            interactionUI.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }
    }
}