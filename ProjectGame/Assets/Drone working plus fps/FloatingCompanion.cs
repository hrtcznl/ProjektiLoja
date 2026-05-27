using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmoothShakeFree;

[RequireComponent(typeof(Rigidbody))]
public class FloatingCompanion : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;
    public DialogueUI dialogueUI;
    public FPSController controller;

    [Header("Dialogue Audio")]
    public AudioSource[] robotTalkAudioSources = new AudioSource[3];
    public int minTalkLoops = 1;
    public int maxTalkLoops = 3;
    private Coroutine talkAudioCoroutine;

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
    public GameObject interactionBlocker; // Disable this to allow interaction after dialogue pauses

    [Header("External Animator")]
    public Animator targetAnimator; // Drag the other object's animator here in the Inspector
    private bool animatorEnabled = false; // Track if animator has been enabled

    [Header("Dialogue")]
    public string currentDialogueKey = "default";
    public Button skipDialogueButton;
    public Button playerResponseButton;
    public TMP_Text playerResponseButtonText;
    public GameObject objectToEnableOnIntroEnd; // Enable this when intro dialogue finishes
    public GameObject objectToDisableAfterFourthIntroResponse; // Disable this after the fourth intro response press
    public GameObject secondDialogueTrigger; // Enable this object to trigger the second dialogue
    public GameObject secondDialogueLine2Object; // Object enabled only during second dialogue line 2
    public GameObject secondDialogueLine3Object; // Object enabled on second dialogue line 3 and stays enabled afterward
    public GameObject secondDialogueLine3DisableObject; // Object disabled on second dialogue line 3 and stays disabled afterward
    public GameObject thirdDialogueTrigger; // Enable this object to trigger the third dialogue
    public List<string> secondRobotLines = new List<string>();
    public List<string> secondPlayerResponses = new List<string>();
    public List<string> thirdRobotLines = new List<string>
    {
        "Now that you have collected enough Z-13, you can finally fuel your ship.",
        "Correct. Unrefined Z-13 is far too unstable to be used directly in conventional engines. However, this laboratory contains a specialized Z-13 processor capable of refining the substance into usable spacecraft fuel.",
        "Partially. Dr. House’s team developed a prototype refinement system shortly before the incident. According to the remaining logs, the processed fuel output was highly successful.",
        "Because the teleporter accident occurred less than two days later.",
        "The processing chamber is located in the Energy Room, but first you might need to calculate the logic output of the processor's controller in the Storage Room."
    };
    public List<string> thirdPlayerResponses = new List<string>
    {
        "I’m not sure my ship is even compatible with raw Z-13.",
        "So the expedition already found a way to stabilize it?",
        "Then why didn’t they ever report it back to Earth...",
        "Right...",
        "Good. Let’s finish what they started. (End dialogue)"
    };
    public GameObject fourthDialogueTrigger; // Enable this object to trigger the fourth dialogue after it becomes active
    public List<string> fourthRobotLines = new List<string>
    {
        "J: The refinement process is complete. Your fuel reserves are now sufficient for interplanetary travel.",
        "P: Then I can finally leave this place.",
        "J: Negative. Your ship’s primary engine was critically damaged during the crash on Cydonia. The fuel alone will not be enough.",
        "P: So I still need an engine...",
        "J: Correct. You should be able to find one on the Storage Room.",
        "P: Guess this planet isn’t done with me yet.",
        "J: Captain Vance... thank you.",
        "P: For what?",
        "J: You completed the work this facility was built for. Dr. House would have wanted the research to survive.",
        "P: Maybe now it finally will.",
        "J: Safe travels, Captain.",
        "P: Goodbye, Jarvis."
    };
    public List<string> fourthPlayerResponses = new List<string>
    {
        "Then I can finally leave this place.",
        "Negative. Your ship’s primary engine was critically damaged during the crash on Cydonia. The fuel alone will not be enough.",
        "So I still need an engine...",
        "Correct. You should be able to find one on the Storage Room.",
        "Guess this planet isn’t done with me yet.",
        "Captain Vance... thank you.",
        "For what?",
        "You completed the work this facility was built for. Dr. House would have wanted the research to survive.",
        "Maybe now it finally will.",
        "Safe travels, Captain.",
        "Goodbye, Jarvis."
    };
    public List<string> introRobotLines = new List<string>
    {
        "Oh, welcome back Dr.House! It has\nbeen a while since we talked!",
        "You are in the Cydonia planet, where you and your team are on an expedition called \"Zallspace\" to research a new substance that... The team should be in the Crew Quarters.",
        "Yes, i was programmed to call you\nDr.House - the Head Researcher of\nthe Cydonia Laboratory!",
        "Hello again Dr.House!",
        "I do not understand. Why are you apologizing, Dr. House?",
        "That is... Oh no... .",
        "14 years, 3 months, 28 days, 6 hours, 42 minutes, 12 seconds. I continued maintenance procedures while awaiting the crew’s return. My external communication systems were damaged during the teleporter incident. I could not access the surface network or the relay satellites.\n\nIf you are not Dr. House... then who are you?",
        "Negative. The laboratory remains operational. Though many sectors are\nnow unstable.",
        "The expedition’s primary objective was the study of a substance designated \"Z-13.\" It was discovered beneath the surface of the nearest planet to Cydonia, Riosar.",
        "According to Dr. House’s research, Z-13 possessed an energy density approximately 67 times greater than liquid hydrogen. If stabilized, it could have become the most efficient spacecraft fuel ever discovered.",
        "Correct. Interstellar travel times would have been reduced dramatically. Entire colonies could have been powered for decades with only a few kilograms of material.",
        "No the teleporter is completely electronic, based on the maintenance logs: a catastrophic power instability occurred during the transfer sequence. The teleporter shut down mid-transport. Recovery attempts failed.",
        "That is the official conclusion\nstated in the emergency logs.",
        "I was unable to access the energy sector after the lockdown. My security clearance was restricted to the central laboratory, and I did not possess the keypad authorization required to enter that area.",
        "Correct. Once you unlocked the sector, I regained access to its maintenance network and diagnostic archives.",
        "Yes. The issue originated from the primary power supply unit connected to the transfer array. However, the damaged power supply can be replaced with a compatible unit for the teleporter to function safely again.",
        "According to inventory records, a backup unit is not available but there are still some components that you can use to make it throughout the laboratory."
    };
    public List<string> introPlayerResponses = new List<string>
    {
        "Where am I? Where is everybody?",
        "Dr.House?",
        "(Go to the Crew Quarters)",
        "I'm sorry...\n(Give note)",
        "I’m not Dr. House. I found this note\nin the crew quarters.",
        "You didn’t know? How long have you been here alone?",
        "Captain Orion Vance. My ship crashed a few kilometers from here. I thought this planet was abandoned.",
        "What exactly were they researching here?",
        "What made it so important?",
        "That would change everything.",
        "So they used it in the teleporter?",
        "And nobody survived...",
        "If the teleporter malfunctioned,\nwhy didn’t you repair it?",
        "But I opened that door earlier.",
        "Can it be fixed?",
        "Where can i find one?",
        "Then that’s what we’re doing.\n(End dialogue)"
    };

    private bool hasSeenIntro = false;
    private bool isIntroDialogueActive = false;
    private bool isIntroDialoguePaused = false;
    private int introLineIndex = 0;
    private bool hasSeenSecondDialogue = false;
    private bool isSecondDialogueActive = false;
    private bool isSecondDialoguePaused = false;
    private int secondLineIndex = 0;
    private bool hasSeenThirdDialogue = false;
    private bool isThirdDialogueActive = false;
    private bool isThirdDialoguePaused = false;
    private int thirdLineIndex = 0;
    private bool hasSeenFourthDialogue = false;
    private bool isFourthDialogueActive = false;
    private bool isFourthDialoguePaused = false;
    private int fourthLineIndex = 0;
    private Rigidbody rb;
    private bool isActive = false;
    private bool isInDialogue = false;
    private Dictionary<string, string> dialogues;
    private bool followWaitForInteraction = false;
    private bool previousInteractionBlocked = false;
    private bool localInteractionBlocked = false; // used if no blocker object is assigned or to avoid toggling the assigned object
    private bool interactionBlockerActivatedByScript = false;

    public bool HasSeenSecondDialogue => hasSeenSecondDialogue;
    public bool HasSeenThirdDialogue => hasSeenThirdDialogue;

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

        if (skipDialogueButton != null)
            skipDialogueButton.gameObject.SetActive(false);

        if (playerResponseButton != null)
            playerResponseButton.gameObject.SetActive(false);

        if (playerResponseButtonText == null && playerResponseButton != null)
            playerResponseButtonText = playerResponseButton.GetComponentInChildren<TMP_Text>();

        minTalkLoops = Mathf.Max(1, minTalkLoops);
        maxTalkLoops = Mathf.Max(minTalkLoops, maxTalkLoops);

        if (secondDialogueLine2Object != null)
            secondDialogueLine2Object.SetActive(false);
        if (secondDialogueLine3Object != null)
            secondDialogueLine3Object.SetActive(false);

        // Keep the blocker object in its scene state, but do not treat it as active until the script enables it.
        localInteractionBlocked = false;
        interactionBlockerActivatedByScript = false;
        previousInteractionBlocked = IsInteractionBlocked();
        followWaitForInteraction = false;
    }

    void Update()
    {
        UpdateBlockerState();
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

        // Allow interaction when the intro is paused at line 3 so the default task menu can still be opened.
        if (IsInteractionBlocked() && !(isIntroDialoguePaused && introLineIndex == 3))
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
        // Allow interaction while blocked if the first intro dialogue is paused at line 3.
        if (IsInteractionBlocked() && !(isIntroDialoguePaused && introLineIndex == 3))
            return false;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, companionLayer))
        {
            return hit.collider != null && hit.collider.transform == transform;
        }
        return false;
    }

    void FollowPlayer()
    {
        // Do not follow while interaction is blocked
        if (IsInteractionBlocked() || followWaitForInteraction)
            return;

        // Target position behind the player
        Vector3 targetPos = player.position - player.forward * followDistance;

        // Set hover height to player's Y position
        targetPos.y = player.position.y + hoverHeight;

        // Compute direction
        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;

        // Cap movement per frame so the companion moves smoothly instead of snapping
        float maxStep = moveSpeed * Time.deltaTime;

        // Move only if beyond stopDistance to avoid clipping into player
        if (distance > stopDistance)
        {
            // Don't overshoot the desired stop distance
            float moveDistance = Mathf.Min(distance - stopDistance, maxStep);
            if (moveDistance > 0f)
            {
                Vector3 move = direction.normalized * moveDistance;
                rb.MovePosition(transform.position + move);
            }
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
        // If interaction is blocked, skip raycast and do nothing unless the first intro dialogue is paused at line 3.
        if (IsInteractionBlocked() && !(isIntroDialoguePaused && introLineIndex == 3))
            return;

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

                if (dialogueUI != null)
                {
                    followWaitForInteraction = false;
                    if (isFourthDialoguePaused)
                        ResumeFourthDialogue();
                    else if (!hasSeenFourthDialogue && hasSeenIntro && fourthDialogueTrigger != null && fourthDialogueTrigger.activeSelf)
                        StartFourthDialogue();
                    else if (isThirdDialoguePaused)
                        ResumeThirdDialogue();
                    else if (!hasSeenThirdDialogue && hasSeenIntro && thirdDialogueTrigger != null && thirdDialogueTrigger.activeSelf)
                        StartThirdDialogue();
                    else if (!hasSeenSecondDialogue && hasSeenIntro && secondDialogueTrigger != null && secondDialogueTrigger.activeSelf)
                        StartSecondDialogue();
                    else if (isIntroDialoguePaused)
                    {
                        if (IsInteractionBlocked() && introLineIndex == 3)
                        {
                            currentDialogueKey = "default";
                            StartDialogue();
                        }
                        else
                        {
                            ResumeIntroDialogue();
                        }
                    }
                    else if (!hasSeenIntro)
                        StartIntroDialogue();
                    else
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
            ShowDialogue(dialogues[currentDialogueKey]);
            isInDialogue = true;
            SetIntroUIActive(false);

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

    void StartIntroDialogue()
    {
        if (dialogueUI == null || isInDialogue)
            return;

        if (introRobotLines == null || introRobotLines.Count == 0)
        {
            hasSeenIntro = true;
            StartDialogue();
            return;
        }

        isInDialogue = true;
        isIntroDialogueActive = true;
        introLineIndex = 0;
        ShowDialogue(introRobotLines[introLineIndex]);

        SetIntroUIActive(true);
        EnableTaskButtons(false);
        EnableBackButton(false);

        if (playerResponseButtonText != null)
        {
            if (introPlayerResponses != null && introPlayerResponses.Count > 0)
                playerResponseButtonText.text = introPlayerResponses[0];
            else
                playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }
    }

    void OnPlayerResponseButtonClicked()
    {
        if (!isIntroDialogueActive)
            return;

        if (introRobotLines == null || introRobotLines.Count == 0)
        {
            FinishIntroDialogue();
            return;
        }

        if (introLineIndex >= introRobotLines.Count - 1)
        {
            FinishIntroDialogue();
            return;
        }

        introLineIndex++;
        ShowDialogue(introRobotLines[introLineIndex]);
        DisableFourthIntroResponseObjectIfNeeded();

        if (playerResponseButtonText != null)
        {
            if (introPlayerResponses != null && introLineIndex < introPlayerResponses.Count)
                playerResponseButtonText.text = introPlayerResponses[introLineIndex];
            else
                playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }

        if (introLineIndex >= introRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }
        // After 3rd button press (introLineIndex == 3), pause and close dialogue only if the blocker is still active
        if (introLineIndex == 3)
        {
            bool blockerDisabled = (interactionBlocker == null || !interactionBlocker.activeSelf);
            if (!blockerDisabled)
            {
                // Blocker is active, pause dialogue and activate interaction blocking
                dialogueUI?.Hide();
                isIntroDialogueActive = false;
                isIntroDialoguePaused = true;
                SetIntroUIActive(false);
                isInDialogue = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.canMove = true;
                
                // Block interaction until blocker is disabled. Prefer toggling the assigned object
                // but avoid toggling it if it's the companion itself or a parent/child to prevent accidental deactivation.
                if (interactionBlocker != null)
                {
                    if (interactionBlocker != this.gameObject && !interactionBlocker.transform.IsChildOf(transform) && !transform.IsChildOf(interactionBlocker.transform))
                    {
                        interactionBlocker.SetActive(true);
                        interactionBlockerActivatedByScript = true;
                    }
                    else
                    {
                        localInteractionBlocked = true;
                    }
                }
                else
                {
                    localInteractionBlocked = true;
                }
            }
        }
    }

    void SkipIntroDialogue()
    {
        if (!isIntroDialogueActive)
            return;

        FinishIntroDialogue();
    }

    void DisableFourthIntroResponseObjectIfNeeded()
    {
        if (introLineIndex == 4 && objectToDisableAfterFourthIntroResponse != null)
        {
            objectToDisableAfterFourthIntroResponse.SetActive(false);
        }
    }

    void ResumeIntroDialogue()
    {
        if (!isIntroDialoguePaused)
            return;

        isIntroDialogueActive = true;
        isIntroDialoguePaused = false;
        isInDialogue = true;
        ShowDialogue(introRobotLines[introLineIndex]);

        SetIntroUIActive(true);

        if (playerResponseButtonText != null)
        {
            if (introPlayerResponses != null && introLineIndex < introPlayerResponses.Count)
                playerResponseButtonText.text = introPlayerResponses[introLineIndex];
            else
                playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }

        if (introLineIndex >= introRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }
    }

    void FinishIntroDialogue()
    {
        hasSeenIntro = true;
        isIntroDialogueActive = false;
        currentDialogueKey = "default";
        ShowDialogue(dialogues[currentDialogueKey]);
        SetIntroUIActive(false);
        EnableTaskButtons(true);
        EnableBackButton(false);

        if (objectToEnableOnIntroEnd != null)
            objectToEnableOnIntroEnd.SetActive(true);
    }

    void StartSecondDialogue()
    {
        if (dialogueUI == null || isInDialogue)
            return;

        if (secondRobotLines == null || secondRobotLines.Count == 0)
        {
            FinishSecondDialogue();
            return;
        }

        isInDialogue = true;
        isSecondDialogueActive = true;
        secondLineIndex = 0;
        ShowDialogue(secondRobotLines[secondLineIndex]);

        SetIntroUIActive(true);
        EnableTaskButtons(false);
        EnableBackButton(false);

        if (playerResponseButtonText != null)
        {
            if (secondPlayerResponses != null && secondPlayerResponses.Count > 0)
                playerResponseButtonText.text = secondPlayerResponses[0];
            else
                playerResponseButtonText.text = "Then that’s what we’re doing.\n(End dialogue)";
        }
    }

    void OnSecondPlayerResponseButtonClicked()
    {
        if (!isSecondDialogueActive)
            return;

        if (secondRobotLines == null || secondRobotLines.Count == 0)
        {
            FinishSecondDialogue();
            return;
        }

        if (secondLineIndex >= secondRobotLines.Count - 1)
        {
            FinishSecondDialogue();
            return;
        }

        secondLineIndex++;
        ShowDialogue(secondRobotLines[secondLineIndex]);

        if (playerResponseButtonText != null)
        {
            if (secondPlayerResponses != null && secondLineIndex < secondPlayerResponses.Count)
                playerResponseButtonText.text = secondPlayerResponses[secondLineIndex];
            else
                playerResponseButtonText.text = "Alright then, let's go get the rover.\n(End dialogue)";
        }

        if (secondLineIndex >= secondRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Alright then, let's go get the rover.\n(End dialogue)";
        }
    }

    void SkipSecondDialogue()
    {
        if (!isSecondDialogueActive)
            return;

        FinishSecondDialogue();
    }

    void ResumeSecondDialogue()
    {
        if (!isSecondDialoguePaused)
            return;

        isSecondDialogueActive = true;
        isSecondDialoguePaused = false;
        isInDialogue = true;
        ShowDialogue(secondRobotLines[secondLineIndex]);

        SetIntroUIActive(true);

        if (playerResponseButtonText != null)
        {
            if (secondPlayerResponses != null && secondLineIndex < secondPlayerResponses.Count)
                playerResponseButtonText.text = secondPlayerResponses[secondLineIndex];
            else
                playerResponseButtonText.text = "Alright then, let's go get the rover.\n(End dialogue)";
        }

        if (secondLineIndex >= secondRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Alright then, let's go get the rover.\n(End dialogue)";
        }
    }

    void FinishSecondDialogue()
    {
        hasSeenSecondDialogue = true;
        isSecondDialogueActive = false;
        currentDialogueKey = "default";
        ShowDialogue(dialogues[currentDialogueKey]);
        SetIntroUIActive(false);
        EnableTaskButtons(true);
        EnableBackButton(false);
    }

    void StartThirdDialogue()
    {
        if (dialogueUI == null || isInDialogue)
            return;

        if (thirdRobotLines == null || thirdRobotLines.Count == 0)
        {
            FinishThirdDialogue();
            return;
        }

        isInDialogue = true;
        isThirdDialogueActive = true;
        thirdLineIndex = 0;
        ShowDialogue(thirdRobotLines[thirdLineIndex]);

        SetIntroUIActive(true);
        EnableTaskButtons(false);
        EnableBackButton(false);

        if (playerResponseButtonText != null)
        {
            if (thirdPlayerResponses != null && thirdPlayerResponses.Count > 0)
                playerResponseButtonText.text = thirdPlayerResponses[0];
            else
                playerResponseButtonText.text = "Good. Let’s finish what they started.\n(End dialogue)";
        }
    }

    void OnThirdPlayerResponseButtonClicked()
    {
        if (!isThirdDialogueActive)
            return;

        if (thirdRobotLines == null || thirdRobotLines.Count == 0)
        {
            FinishThirdDialogue();
            return;
        }

        if (thirdLineIndex >= thirdRobotLines.Count - 1)
        {
            FinishThirdDialogue();
            return;
        }

        thirdLineIndex++;
        ShowDialogue(thirdRobotLines[thirdLineIndex]);

        if (playerResponseButtonText != null)
        {
            if (thirdPlayerResponses != null && thirdLineIndex < thirdPlayerResponses.Count)
                playerResponseButtonText.text = thirdPlayerResponses[thirdLineIndex];
            else
                playerResponseButtonText.text = "Good. Let’s finish what they started.\n(End dialogue)";
        }

        if (thirdLineIndex >= thirdRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Good. Let’s finish what they started.\n(End dialogue)";
        }
    }

    void SkipThirdDialogue()
    {
        if (!isThirdDialogueActive)
            return;

        FinishThirdDialogue();
    }

    void ResumeThirdDialogue()
    {
        if (!isThirdDialoguePaused)
            return;

        isThirdDialogueActive = true;
        isThirdDialoguePaused = false;
        isInDialogue = true;
        ShowDialogue(thirdRobotLines[thirdLineIndex]);

        SetIntroUIActive(true);

        if (playerResponseButtonText != null)
        {
            if (thirdPlayerResponses != null && thirdLineIndex < thirdPlayerResponses.Count)
                playerResponseButtonText.text = thirdPlayerResponses[thirdLineIndex];
            else
                playerResponseButtonText.text = "Good. Let’s finish what they started.\n(End dialogue)";
        }

        if (thirdLineIndex >= thirdRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Good. Let’s finish what they started.\n(End dialogue)";
        }
    }

    void FinishThirdDialogue()
    {
        hasSeenThirdDialogue = true;
        isThirdDialogueActive = false;
        currentDialogueKey = "default";
        ShowDialogue(dialogues[currentDialogueKey]);
        SetIntroUIActive(false);
        EnableTaskButtons(true);
        EnableBackButton(false);
    }

    void StartFourthDialogue()
    {
        if (dialogueUI == null || isInDialogue)
            return;

        if (fourthRobotLines == null || fourthRobotLines.Count == 0)
        {
            FinishFourthDialogue();
            return;
        }

        isInDialogue = true;
        isFourthDialogueActive = true;
        fourthLineIndex = 0;
        ShowDialogue(fourthRobotLines[fourthLineIndex]);

        SetIntroUIActive(true);
        EnableTaskButtons(false);
        EnableBackButton(false);

        if (playerResponseButtonText != null)
        {
            if (fourthPlayerResponses != null && fourthPlayerResponses.Count > 0)
                playerResponseButtonText.text = fourthPlayerResponses[0];
            else
                playerResponseButtonText.text = "Goodbye, Jarvis.";
        }
    }

    void OnFourthPlayerResponseButtonClicked()
    {
        if (!isFourthDialogueActive)
            return;

        if (fourthRobotLines == null || fourthRobotLines.Count == 0)
        {
            FinishFourthDialogue();
            return;
        }

        if (fourthLineIndex >= fourthRobotLines.Count - 1)
        {
            FinishFourthDialogue();
            return;
        }

        fourthLineIndex++;
        ShowDialogue(fourthRobotLines[fourthLineIndex]);

        if (playerResponseButtonText != null)
        {
            if (fourthPlayerResponses != null && fourthLineIndex < fourthPlayerResponses.Count)
                playerResponseButtonText.text = fourthPlayerResponses[fourthLineIndex];
            else
                playerResponseButtonText.text = "Goodbye, Jarvis.";
        }

        if (fourthLineIndex >= fourthRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Goodbye, Jarvis.";
        }
    }

    void SkipFourthDialogue()
    {
        if (!isFourthDialogueActive)
            return;

        FinishFourthDialogue();
    }

    void ResumeFourthDialogue()
    {
        if (!isFourthDialoguePaused)
            return;

        isFourthDialogueActive = true;
        isFourthDialoguePaused = false;
        isInDialogue = true;
        ShowDialogue(fourthRobotLines[fourthLineIndex]);

        SetIntroUIActive(true);

        if (playerResponseButtonText != null)
        {
            if (fourthPlayerResponses != null && fourthLineIndex < fourthPlayerResponses.Count)
                playerResponseButtonText.text = fourthPlayerResponses[fourthLineIndex];
            else
                playerResponseButtonText.text = "Goodbye, Jarvis.";
        }

        if (fourthLineIndex >= fourthRobotLines.Count - 1)
        {
            playerResponseButtonText.text = "Goodbye, Jarvis.";
        }
    }

    void FinishFourthDialogue()
    {
        hasSeenFourthDialogue = true;
        isFourthDialogueActive = false;
        currentDialogueKey = "default";
        ShowDialogue(dialogues[currentDialogueKey]);
        SetIntroUIActive(false);
        EnableTaskButtons(true);
        EnableBackButton(false);
    }

    void HandleDialogueClose()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(interactKey))
        {
            dialogueUI?.Hide();
            StopTalkAudio();
            isInDialogue = false;

            if (isThirdDialogueActive && !hasSeenThirdDialogue)
            {
                // Pause third dialogue and keep progress for next access
                isThirdDialogueActive = false;
                isThirdDialoguePaused = true;
                SetIntroUIActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.canMove = true;
                return;
            }

            if (isSecondDialogueActive && !hasSeenSecondDialogue)
            {
                // Pause second dialogue and keep progress for next access
                isSecondDialogueActive = false;
                isSecondDialoguePaused = true;
                UpdateSecondDialogueLine2Object();
                SetIntroUIActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.canMove = true;
                return;
            }

            if (isIntroDialogueActive && !hasSeenIntro)
            {
                // Pause initial intro dialogue and keep progress for next access
                isIntroDialogueActive = false;
                isIntroDialoguePaused = true;
                SetIntroUIActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.canMove = true;
                return;
            }

            if (isIntroDialoguePaused && introLineIndex == 3 && !isIntroDialogueActive)
            {
                // Keep the intro paused when closing the default task UI during the blocked pause.
                SetIntroUIActive(false);
                currentDialogueKey = "default";
                EnableTaskButtons(false);
                EnableBackButton(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.canMove = true;
                return;
            }

            isIntroDialogueActive = false;
            isIntroDialoguePaused = false;
            isSecondDialogueActive = false;
            isSecondDialoguePaused = false;
            SetIntroUIActive(false);
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

    bool IsInteractionBlocked()
    {
        return localInteractionBlocked || (interactionBlocker != null && interactionBlocker.activeSelf && interactionBlockerActivatedByScript);
    }

    void UpdateBlockerState()
    {
        bool currentlyBlocked = IsInteractionBlocked();
        if (previousInteractionBlocked && !currentlyBlocked)
        {
            followWaitForInteraction = true;
        }
        previousInteractionBlocked = currentlyBlocked;
    }

    void InitializeDialogue()
    {
        dialogues = new Dictionary<string, string>
        {
            { "default", "Hey there. Tell me if you need\nhelp with anything:\n\n\n\n\n" },
            { "task_1", "Read left to right: the first three colors are digits, the fourth color tells how many zeros to add (multiplier), and the last band shows accuracy in percentage. Refer to the color chart in the energy room." },
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
        if (playerResponseButton != null)
            playerResponseButton.onClick.AddListener(OnResponseButtonClicked);
        if (skipDialogueButton != null)
            skipDialogueButton.onClick.AddListener(OnSkipDialogueButtonClicked);
    }

    void OnResponseButtonClicked()
    {
        if (isFourthDialogueActive)
            OnFourthPlayerResponseButtonClicked();
        else if (isThirdDialogueActive)
            OnThirdPlayerResponseButtonClicked();
        else if (isSecondDialogueActive)
            OnSecondPlayerResponseButtonClicked();
        else
            OnPlayerResponseButtonClicked();
    }

    void OnSkipDialogueButtonClicked()
    {
        if (isFourthDialogueActive)
            SkipFourthDialogue();
        else if (isThirdDialogueActive)
            SkipThirdDialogue();
        else if (isSecondDialogueActive)
            SkipSecondDialogue();
        else
            SkipIntroDialogue();
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
            ShowDialogue(dialogues[taskKey]);
            EnableTaskButtons(false);
            EnableBackButton(true);
        }
    }

    void EnableBackButton(bool enabled)
    {
        if (backButton != null)
            backButton.gameObject.SetActive(enabled);
    }

    void SetIntroUIActive(bool enabled)
    {
        if (skipDialogueButton != null)
            skipDialogueButton.gameObject.SetActive(enabled);
        if (playerResponseButton != null)
            playerResponseButton.gameObject.SetActive(enabled);
    }

    void OnBackButtonClicked()
    {
        if (dialogueUI != null)
        {
            currentDialogueKey = "default";
            StopTalkAudio();
            dialogueUI.Show(dialogues["default"]);
            UpdateSecondDialogueLine2Object();
            EnableTaskButtons(true);
            EnableBackButton(false);
        }
    }

    void ShowDialogue(string text)
    {
        dialogueUI.Show(text);
        PlayTalkAudio();
        UpdateSecondDialogueLine2Object();
    }

    void UpdateSecondDialogueLine2Object()
    {
        if (secondDialogueLine2Object != null)
        {
            secondDialogueLine2Object.SetActive(isSecondDialogueActive && secondLineIndex == 1);
        }

        if (secondDialogueLine3Object != null && isSecondDialogueActive && secondLineIndex == 2)
        {
            secondDialogueLine3Object.SetActive(true);
        }

        if (secondDialogueLine3DisableObject != null && isSecondDialogueActive && secondLineIndex == 2)
        {
            secondDialogueLine3DisableObject.SetActive(false);
        }
    }

    void PlayTalkAudio()
    {
        if (robotTalkAudioSources == null || robotTalkAudioSources.Length == 0)
            return;

        List<AudioSource> validSources = new List<AudioSource>();
        foreach (var source in robotTalkAudioSources)
        {
            if (source != null && source.clip != null)
                validSources.Add(source);
        }

        if (validSources.Count == 0)
            return;

        StopTalkAudio();
        int loops = Random.Range(minTalkLoops, maxTalkLoops + 1);
        talkAudioCoroutine = StartCoroutine(PlayTalkAudioLoop(validSources, loops));
    }

    System.Collections.IEnumerator PlayTalkAudioLoop(List<AudioSource> sources, int loops)
    {
        for (int i = 0; i < loops; i++)
        {
            AudioSource source = sources[Random.Range(0, sources.Count)];
            source.loop = false;
            source.Play();

            float duration = source.clip != null ? source.clip.length : 0f;
            if (duration <= 0f)
                yield break;

            yield return new WaitForSecondsRealtime(duration);
        }

        StopTalkAudio();
    }

    void StopTalkAudio()
    {
        if (talkAudioCoroutine != null)
        {
            StopCoroutine(talkAudioCoroutine);
            talkAudioCoroutine = null;
        }

        if (robotTalkAudioSources != null)
        {
            foreach (var source in robotTalkAudioSources)
            {
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
    }
}