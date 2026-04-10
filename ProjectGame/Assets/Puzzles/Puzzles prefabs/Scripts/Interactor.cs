using UnityEngine;
using TMPro;

public interface IInteractable
{
    void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 5f;

    [Header("UI Prompt")]
    public TMP_Text interactPromptText;

    void Start()
    {
        if (InteractorSource == null && Camera.main != null)
        {
            InteractorSource = Camera.main.transform;
        }

        HidePrompt();
    }

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void CheckForInteractable()
    {
        if (InteractorSource == null)
        {
            HidePrompt();
            return;
        }

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                ShowPrompt();
                return;
            }
        }

        HidePrompt();
    }

    void TryInteract()
    {
        if (InteractorSource == null)
            return;

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
            }
        }
    }

    void ShowPrompt()
    {
        if (interactPromptText != null)
            interactPromptText.gameObject.SetActive(true);
    }

    void HidePrompt()
    {
        if (interactPromptText != null)
            interactPromptText.gameObject.SetActive(false);
    }
}