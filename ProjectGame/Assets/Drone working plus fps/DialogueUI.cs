using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References (optional)")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public FPSController playerController;

    private bool isOpen = false;

    void Start()
    {
        // Auto-find if not assigned
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("DialoguePannel"); // Make sure name matches exactly
            if (dialoguePanel == null)
                Debug.LogError("DialoguePanel not assigned or found!");
        }

        if (dialogueText == null)
        {
            GameObject textObj = GameObject.Find("DialogueText");
            if (textObj != null)
                dialogueText = textObj.GetComponent<TMP_Text>();
            else
                Debug.LogError("DialogueText not assigned or found!");
        }

        if (playerController == null)
            playerController = FindObjectOfType<FPSController>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void Show(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        isOpen = true;
        if (playerController != null)
            playerController.canMove = false;
    }

    public void Hide()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isOpen = false;
        if (playerController != null)
            playerController.canMove = true;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
