using UnityEngine;

public class Switch : InteractableBlock
{
    [Header("!!!!Switches should be set to Repeat!!!!")] 
    [SerializeField] private GameObject linkedBlock;
    [SerializeField] SwitchMode switchMode;
    [SerializeField] KeyCode interactionKey = KeyCode.E; // Default interaction key is 'E'
    private enum SwitchMode
    {
        toggleShine,
        toggleMovement,
        toggleDisappear
    }

    public override void Interact()
    {
        if (switchMode == SwitchMode.toggleShine)
        {
            toggleLinkedBlockShine();
        }
        else if (switchMode == SwitchMode.toggleMovement)
        {
            toggleLinkedBlockMovement();
        }
        else if (switchMode == SwitchMode.toggleDisappear)
        {
            toggleExistance(); // Note: This mode is not fully implemented yet
        }
    }
    
    private void toggleLinkedBlockShine()
    {
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<InteractableBlock>() && linkedBlock.GetComponent<InteractableBlock>().isVisible())
        {
            linkedBlock.GetComponent<InteractableBlock>().shineBlock();
        }
        else if (linkedBlock.GetComponent<InteractableBlock>() && !linkedBlock.GetComponent<InteractableBlock>().isVisible())
        {
            linkedBlock.GetComponent<InteractableBlock>().deshineBlock();
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    private void toggleExistance()
    {
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<InteractableBlock>() && linkedBlock.GetComponent<InteractableBlock>().isVisible())
        {
            InteractableBlock.disableBlock(linkedBlock);
        }
        else if (linkedBlock.GetComponent<InteractableBlock>() && !linkedBlock.GetComponent<InteractableBlock>().isVisible())
        {
            InteractableBlock.enableBlock(linkedBlock);
            //linkedBlock.GetComponentInChildren<InteractableBlock>().shineBlock(); // Also shine for its light form if it has one
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    private void toggleLinkedBlockMovement()
    {
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<InteractableBlock>() && linkedBlock.GetComponent<InteractableBlock>().isMovable())
        {
            linkedBlock.GetComponent<InteractableBlock>().makeImmovable();
            //linkedBlock.GetComponentInChildren<InteractableBlock>().makeImmovable(); // Also make immovable for its light form if it has one
        }
        else if (linkedBlock.GetComponent<InteractableBlock>() && !linkedBlock.GetComponent<InteractableBlock>().isMovable())
        {
            linkedBlock.GetComponent<InteractableBlock>().makeMovable();
            //linkedBlock.GetComponentInChildren<InteractableBlock>().makeMovable(); // Also make immovable for its light form if it has one
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.init();
        //makeMovable(); // Switches are not movable
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.L))
        {
            shineBlock();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            deshineBlock();
        }
        if (Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }
    
}
