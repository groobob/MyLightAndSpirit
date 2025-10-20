using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public DialogueLine[] dialogueLines;
    [Header("Character Portraits")]
    [SerializeField] private List<Sprite> portraits;

    private int currentLineIndex = 0;
    [SerializeField] private DialogueUI dialogueUI;
    private bool isDialogueActive = false;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                dialogueUI.FinishTyping();
                isTyping = false;
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] newDialogue)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("Dialogue already active. Ignoring new request.");
            return;
        }

        if (newDialogue == null || newDialogue.Length == 0)
        {
            Debug.LogWarning("Empty dialogue.");
            return;
        }

        dialogueLines = newDialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        dialogueUI.Show();
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            DialogueLine line = dialogueLines[currentLineIndex];
            currentLineIndex++;
            isTyping = true;

            dialogueUI.SetSpeaker(line.speakerName, line.portrait);
            dialogueUI.TypeLine(line.text, () => isTyping = false);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.Hide();
    }

    public bool IsDialogueActive() => isDialogueActive;

    public Sprite GetPortrait(int index)
    {
        // 0 is "None", so we skip that
        if (index < 0 || index >= portraits.Count)
        {
            Debug.LogWarning($"No portrait found for (index {index}).");
            return null;
        }

        return portraits[index];
    }

}
