using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 2f; // Radius for interaction
    [SerializeField] private GameObject interactIndicator; // The "Press F to Interact" indicator
    private bool isPlayerInRange = false;                 // Tracks if player is in range

    [Header("Dialogue Settings")]
    [SerializeField] private List<DialogueLine> dialogueLines; // List of dialogue lines
    [SerializeField] private GameObject dialogueUI;            // Reference to the dialogue UI
    [SerializeField] private TextMeshProUGUI speakerText;       // UI for speaker name
    [SerializeField] private TextMeshProUGUI dialogueText;      // UI for dialogue content
    [SerializeField] private Image speakerImageA;              // Image for Speaker A
    [SerializeField] private Image speakerImageB;              // Image for Speaker B

    private Queue<DialogueLine> dialogueQueue; // Queue to manage dialogue lines
    private bool isDialogueActive = false;     // Tracks if dialogue is active

    private void Start()
    {
        dialogueQueue = new Queue<DialogueLine>();
        dialogueUI.SetActive(false); // Ensure dialogue UI is hidden initially

        // Ensure the interact indicator is hidden initially
        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false);
        }

        speakerImageA.enabled = false;
        speakerImageB.enabled = false;
    }

    private void Update()
    {
        // Check for interaction input when the player is in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogueLines.Count == 0)
        {
            Debug.LogWarning("No dialogue lines assigned for this NPC!");
            return;
        }

        Debug.Log("Dialogue started.");
        isDialogueActive = true;
        dialogueQueue.Clear();

        foreach (var line in dialogueLines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialogueUI.SetActive(true); // Show dialogue UI

        // Hide the interact indicator when dialogue starts
        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false);
        }

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        var currentLine = dialogueQueue.Dequeue();

        // Update the speaker's name and text
        speakerText.text = currentLine.SpeakerName;
        dialogueText.text = currentLine.DialogueText;

        // Show Speaker A's image if assigned
        if (currentLine.SpeakerImageA != null)
        {
            speakerImageA.sprite = currentLine.SpeakerImageA;
            speakerImageA.enabled = true;
        }
        else
        {
            speakerImageA.enabled = false; // Hide Speaker A if no image
        }

        // Show Speaker B's image if assigned
        if (currentLine.SpeakerImageB != null)
        {
            speakerImageB.sprite = currentLine.SpeakerImageB;
            speakerImageB.enabled = true;
        }
        else
        {
            speakerImageB.enabled = false; // Hide Speaker B if no image
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue ended.");
        isDialogueActive = false;
        dialogueUI.SetActive(false); // Hide dialogue UI

        // Clear the UI text
        speakerText.text = "";
        dialogueText.text = "";

        // Hide both speaker images
        speakerImageA.enabled = false;
        speakerImageB.enabled = false;

        // Show the interact indicator again
        if (interactIndicator != null && isPlayerInRange)
        {
            interactIndicator.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered interaction range.");

            // Show the interact indicator
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left interaction range.");

            // Hide the interact indicator
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
