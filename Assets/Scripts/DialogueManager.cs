using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;   // Text field for speaker's name
    [SerializeField] private TextMeshProUGUI dialogueText;  // Text field for dialogue content
    [SerializeField] private GameObject dialogueUI;         // Parent object of the dialogue UI

    [SerializeField] private Image speakerImageA;           // Image for the first character (Speaker A)
    [SerializeField] private Image speakerImageB;           // Image for the second character (Speaker B)

    private Queue<DialogueLine> sentences;  // Queue to store dialogue lines
    private bool isDialogueActive = false;

    public static DialogueManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sentences = new Queue<DialogueLine>();
        dialogueUI.SetActive(false);  // Ensure dialogue UI is hidden initially
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Dialogue started.");
        isDialogueActive = true;
        dialogueUI.SetActive(true);  // Show dialogue UI

        sentences.Clear();

        foreach (DialogueLine line in dialogue.DialogueLines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = sentences.Dequeue();

        // Set the speaker's name and text
        speakerText.text = currentLine.SpeakerName;
        dialogueText.text = currentLine.DialogueText;

        // Logic to show only one speaker image at a time
        if (currentLine.SpeakerImageA != null)
        {
            // Speaker A is talking, show SpeakerImageA and hide SpeakerImageB
            speakerImageA.sprite = currentLine.SpeakerImageA;
            speakerImageA.enabled = true;  // Enable Speaker A's image
            speakerImageB.enabled = false; // Hide Speaker B's image
        }
        else if (currentLine.SpeakerImageB != null)
        {
            // Speaker B is talking, show SpeakerImageB and hide SpeakerImageA
            speakerImageB.sprite = currentLine.SpeakerImageB;
            speakerImageB.enabled = true;  // Enable Speaker B's image
            speakerImageA.enabled = false; // Hide Speaker A's image
        }
        else
        {
            // If no images are provided, hide both speaker images
            speakerImageA.enabled = false;
            speakerImageB.enabled = false;
        }

        Debug.Log($"{currentLine.SpeakerName}: {currentLine.DialogueText}");
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue ended.");
        isDialogueActive = false;
        dialogueUI.SetActive(false);  // Hide dialogue UI when done
        speakerText.text = "";
        dialogueText.text = "";
        speakerImageA.enabled = false;  // Hide both images after dialogue ends
        speakerImageB.enabled = false;
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentence();
        }
    }
}
