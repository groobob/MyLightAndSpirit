using UnityEngine;

public class NPCBlock : InteractableBlock
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private DialogueLine[] setDialogue;
    

    void Start()
    {
        base.init();
        dialogueLines = new DialogueLine[]
        {
            new DialogueLine { text = "This line has no name or portrait." },
            new DialogueLine { speakerName = "Alice", text = "But this one does!", portrait = DialogueManager.Instance.GetComponent<DialogueManager>().GetPortrait(0) },
            new DialogueLine { speakerName = "", text = "Now back to anonymous dialogue." }
        };
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        //testingCode()
    }

    public override void ShineInteract()
    {
        //Do nothing, it's a wall
    }

    /**
    * Abstract method to define interaction behavior. 
    */
    public override void ShineDeinteract()
    {
        // Do nothing, it's a wall
    }
}
