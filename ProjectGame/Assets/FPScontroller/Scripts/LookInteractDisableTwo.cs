using UnityEngine;
using UnityEngine.UI;

public class LookInteractDisableTwo : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public float holdDuration = 1f;

    [Header("References")]
    public Camera playerCamera;
    public GameObject interactionUI;
    public GameObject holdLabel;
    public Slider holdSlider;

    [Header("Objects to Disable on Interact")]
    public GameObject objectToDisable1;
    public GameObject objectToDisable2;

    private bool isLookingAt;
    private float holdTimer;

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (holdSlider != null)
        {
            holdSlider.minValue = 0f;
            holdSlider.maxValue = 1f;
            holdSlider.value = 0f;
        }
    }

    void Update()
    {
        CheckLook();

        if (Input.GetKeyDown(interactKey) && interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (!CanInteract())
        {
            ResetHold();
            return;
        }

        if (isLookingAt && Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;
            float normalizedProgress = holdDuration > 0f ? holdTimer / holdDuration : 1f;

            if (holdSlider != null)
            {
                holdSlider.value = Mathf.Clamp01(normalizedProgress);
            }

            if (holdLabel != null)
            {
                holdLabel.SetActive(true);
            }

            if (holdTimer >= holdDuration)
            {
                Interact();
            }
        }
        else
        {
            ResetHold();
        }
    }

    void CheckLook()
    {
        isLookingAt = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform && CanInteract())
            {
                isLookingAt = true;
            }
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(isLookingAt);
        }
    }

    bool CanInteract()
    {
        return objectToDisable1 != null && objectToDisable1.activeInHierarchy &&
               objectToDisable2 != null && objectToDisable2.activeInHierarchy;
    }

    void ResetHold()
    {
        if (holdTimer == 0f)
        {
            return;
        }

        holdTimer = 0f;

        if (holdSlider != null)
        {
            holdSlider.value = 0f;
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }
    }

    void Interact()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (holdLabel != null)
        {
            holdLabel.SetActive(false);
        }

        if (objectToDisable1 != null)
        {
            objectToDisable1.SetActive(false);
        }

        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
        }
    }
}