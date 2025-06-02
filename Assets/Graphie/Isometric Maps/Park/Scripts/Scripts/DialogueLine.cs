using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [SerializeField] private string speakerName;           // Name of the speaker
    [SerializeField][TextArea(3, 10)] private string dialogueText; // Dialogue text
    [SerializeField] private Sprite speakerImageA;         // Image for Speaker A
    [SerializeField] private Sprite speakerImageB;         // Image for Speaker B
    [SerializeField] private bool isSpeakerAActive;        // Is Speaker A the active speaker?

    public string SpeakerName => speakerName;
    public string DialogueText => dialogueText;
    public Sprite SpeakerImageA => speakerImageA;
    public Sprite SpeakerImageB => speakerImageB;
    public bool IsSpeakerAActive => isSpeakerAActive;
}


[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private List<DialogueLine> dialogueLines;  // List of dialogue lines

    // Public property to access the dialogue lines
    public List<DialogueLine> DialogueLines => dialogueLines;
}
