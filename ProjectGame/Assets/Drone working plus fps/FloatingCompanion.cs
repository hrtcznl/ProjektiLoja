using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingCompanion : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;
    public DialogueUI dialogueUI;
	public FPSController controller;

    [Header("Hover Settings")]
    public float hoverHeight = 1.3f;         // Height above ground
    public float followDistance = 2.5f;      // Desired distance behind player
    public float stopDistance = 4.5f;        // Minimum distance to player
    public float moveSpeed = 15f;             // Max movement speed
    public float rotationSpeed = 15f;         // How fast it rotates to face player
    public float modelForwardCorrection = 180f; // Fix model facing

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    public LayerMask companionLayer;

    [Header("Dialogue")]
    public string currentDialogueKey = "default";

    private Rigidbody rb;
    private bool isActive = false;
    private bool isInDialogue = false;
    private Dictionary<string, string> dialogues;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // kinematic movement, no pushing

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;

        InitializeDialogue();
    }

    void Update()
    {
        HandleActivation();

        if (!isActive || player == null)
            return;

        if (!isInDialogue)
        {
            FollowPlayer();
            TryStartDialogue();
        }
        else
        {
            HandleDialogueClose();
        }
    }

    void HandleActivation()
    {
        if (isActive)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, companionLayer))
            {
                if (hit.collider != null && hit.collider.transform == transform)
                {
                    isActive = true;
                }
            }
        }
    }

    void FollowPlayer()
    {
        // Target position behind the player
        Vector3 targetPos = player.position - player.forward * followDistance;

        // Hover over ground using raycast
        Ray groundRay = new Ray(targetPos + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(groundRay, out RaycastHit hitInfo, 20f))
        {
            targetPos.y = Mathf.Max(hitInfo.point.y + hoverHeight, player.position.y + hoverHeight);
        }
        else
        {
            targetPos.y = player.position.y + hoverHeight;
        }

        // Compute direction
        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;

        // Move only if beyond stopDistance to avoid clipping into player
        if (distance > stopDistance)
        {
            Vector3 move = direction.normalized * Mathf.Min(distance, moveSpeed * Time.deltaTime);
            rb.MovePosition(transform.position + move);
        }

        // Smooth rotation toward player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            targetRot *= Quaternion.Euler(0f, modelForwardCorrection, 0f);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime));
        }
    }

    void TryStartDialogue()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, companionLayer))
        {
            if (hit.collider != null && hit.collider.transform == transform)
            {
                if (dialogueUI != null && dialogues.ContainsKey(currentDialogueKey)){
                    StartDialogue();
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					controller.canMove = false;
				}
				
            }
		}
    }

    void StartDialogue()
    {
        if (dialogueUI != null && !isInDialogue)
        {
            dialogueUI.Show(dialogues[currentDialogueKey]);
            isInDialogue = true;
        }
    }

    void HandleDialogueClose()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(interactKey))
        {
            dialogueUI?.Hide();
            isInDialogue = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			controller.canMove = true;
        }
    }

    void InitializeDialogue()
    {
        dialogues = new Dictionary<string, string>
        {
            { "default", "Hello! I’m your companion." },
            { "task_1", "Look around carefully. The answer is nearby." },
            { "task_2", "This will not respond to force alone." }
        };
    }

    public void SetDialogue(string key)
    {
        if (dialogues.ContainsKey(key))
            currentDialogueKey = key;
    }
}