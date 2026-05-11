using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmoothShakeFree;

[RequireComponent(typeof(Rigidbody))]
public class FloatingCompanion : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;
    public DialogueUI dialogueUI;
    public FPSController controller;

    [Header("Task Dialogue Buttons")]
    public Button taskButton1;
    public Button taskButton2;
    public Button taskButton3;
    public Button taskButton4;
    public Button taskButton5;
    public Button backButton;

    [Header("Shake")]
    [SerializeField] private SmoothShake shake; // <-- drag child SmoothShake here

    [Header("Hover Settings")]
    public float hoverHeight = 1.3f;         // Height above ground
    public float followDistance = 2.5f;      // Desired distance behind player
    public float stopDistance = 4.5f;        // Minimum distance to player
    public float moveSpeed = 15f;            // Max movement speed
    public float rotationSpeed = 15f;        // How fast it rotates to face player
    public float modelForwardCorrection = 180f; // Fix model facing

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    public LayerMask companionLayer;
    public GameObject interactIndicator; // Object to show "Press E" or similar

    [Header("External Animator")]
    public Animator targetAnimator; // Drag the other object's animator here in the Inspector
    private bool animatorEnabled = false; // Track if animator has been enabled

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

        if (interactIndicator != null)
            interactIndicator.SetActive(false);

        InitializeDialogue();
        SetupButtonListeners();
    }

    void Update()
    {
        HandleActivation();

        if (interactIndicator != null)
        {
            interactIndicator.SetActive(!isInDialogue && CanInteract());
        }

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

    bool CanInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, companionLayer))
        {
            return hit.collider != null && hit.collider.transform == transform;
        }
        return false;
    }

    void FollowPlayer()
    {
        // Target position behind the player
        Vector3 targetPos = player.position - player.forward * followDistance;

        // Set hover height to player's Y position
        targetPos.y = player.position.y + hoverHeight;

        // Compute direction
        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;
		float moveSpeed = 15f * distance * distance * distance * distance * distance * distance * distance * 0.00000005f;

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
                // Enable target animator on first interaction
                if (!animatorEnabled && targetAnimator != null)
                {
                    targetAnimator.enabled = true;
                    animatorEnabled = true;
                }

                if (dialogueUI != null && dialogues.ContainsKey(currentDialogueKey))
                {
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

            // Enable task buttons only for default dialogue
            if (currentDialogueKey == "default")
            {
                EnableTaskButtons(true);
                EnableBackButton(false);
            }
            else
            {
                // Enable back button for task dialogues
                EnableTaskButtons(false);
                EnableBackButton(true);
            }
        }
    }

    void HandleDialogueClose()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(interactKey))
        {
            dialogueUI?.Hide();
            isInDialogue = false;
            currentDialogueKey = "default";
            EnableTaskButtons(false);
            EnableBackButton(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controller.canMove = true;

            // ✅ START SHAKE HERE
            shake?.StartShake();
        }
    }

    void InitializeDialogue()
    {
        dialogues = new Dictionary<string, string>
        {
            { "default", "Hello! I am your companion. I am going to help you with your tasks. Select a button:" },
            { "task_1", "Read left to right: the first two colors are digits, the third color tells how many zeros to add (multiplier), and the last band shows accuracy in percentage. Refer to the color chart in the energy room." },
            { "task_2", "Add up resistance differently depending on how they are connected: in series, you just add all resistor values; in parallel, you add the reciprocals (1/R) of each resistor, then take the reciprocal of that result." },
            { "task_3", "Start from 32 bits: the CIDR number tells how many bits are set to 1, so fill that many 1s from left to right, then split into 4 groups of 8 bits and convert each group to decimal to get the subnet mask." },
            { "task_4", "Start from the inputs and go step by step through the circuit: a NOT gate flips the value, an AND gate outputs 1 only if all its inputs are 1, and an OR gate outputs 1 if at least one input is 1. Follow the connections to the output." },
            { "task_5", "Read from right to left: each position is a power of 2 (1, 2, 4, 8, 16, 32, 64, 128), and you add up the values where the bit is 1 to get the decimal number." }
        };
    }

    public void SetDialogue(string key)
    {
        if (dialogues.ContainsKey(key))
            currentDialogueKey = key;
    }

    void SetupButtonListeners()
    {
        if (taskButton1 != null)
            taskButton1.onClick.AddListener(() => OnTaskButtonClicked("task_1"));
        if (taskButton2 != null)
            taskButton2.onClick.AddListener(() => OnTaskButtonClicked("task_2"));
        if (taskButton3 != null)
            taskButton3.onClick.AddListener(() => OnTaskButtonClicked("task_3"));
        if (taskButton4 != null)
            taskButton4.onClick.AddListener(() => OnTaskButtonClicked("task_4"));
        if (taskButton5 != null)
            taskButton5.onClick.AddListener(() => OnTaskButtonClicked("task_5"));
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void EnableTaskButtons(bool enabled)
    {
        if (taskButton1 != null)
            taskButton1.gameObject.SetActive(enabled);
        if (taskButton2 != null)
            taskButton2.gameObject.SetActive(enabled);
        if (taskButton3 != null)
            taskButton3.gameObject.SetActive(enabled);
        if (taskButton4 != null)
            taskButton4.gameObject.SetActive(enabled);
        if (taskButton5 != null)
            taskButton5.gameObject.SetActive(enabled);
    }

    void OnTaskButtonClicked(string taskKey)
    {
        if (dialogueUI != null && dialogues.ContainsKey(taskKey))
        {
            currentDialogueKey = taskKey;
            dialogueUI.Show(dialogues[taskKey]);
            EnableTaskButtons(false);
            EnableBackButton(true);
        }
    }

    void EnableBackButton(bool enabled)
    {
        if (backButton != null)
            backButton.gameObject.SetActive(enabled);
    }

    void OnBackButtonClicked()
    {
        if (dialogueUI != null)
        {
            currentDialogueKey = "default";
            dialogueUI.Show(dialogues["default"]);
            EnableTaskButtons(true);
            EnableBackButton(false);
        }
    }
}