using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : InteractableBlock
{
    [Header("Switch Mode & Linked Block")]
    [SerializeField] protected GameObject linkedBlock;
    [SerializeField] protected SwitchMode switchMode;
    protected enum SwitchMode
    {
        ToggleShine,
        ToggleMovement,
        ToggleDisappear,
        ToggleAppear
    }

    [Header("!!!!Switches should be set to Repeat!!!!")] 
    
    [SerializeField] protected Sprite onSprite;
    [SerializeField] protected Sprite offSprite;
    protected bool onCD = false; // To prevent rapid toggling 
    protected float toggleCD = 0.3f; // Cooldown duration in seconds

    public override void ShineInteract()
    {
        if (!checkSwitchCD()) return;
        spriteRenderer.sprite = onSprite;
        if (switchMode == SwitchMode.ToggleShine)
        {
            toggleLinkedBlockShine();
        }
        else if (switchMode == SwitchMode.ToggleMovement)
        {
            toggleLinkedBlockMovement();
        }
        else if (switchMode == SwitchMode.ToggleDisappear)
        {
            toggleDisappear(); // Note: This mode is not fully implemented yet
        }
        else if (switchMode == SwitchMode.ToggleAppear)
        {
            toggleAppear(); // Note: This mode is not fully implemented yet
        }
    }

    public override void ShineDeinteract()
    {
        spriteRenderer.sprite = offSprite;
        if (switchMode == SwitchMode.ToggleShine)
        {
            toggleLinkedBlockShine();
        }
        else if (switchMode == SwitchMode.ToggleMovement)
        {
            toggleLinkedBlockMovement();
        }
        else if (switchMode == SwitchMode.ToggleDisappear)
        {
            toggleDisappearOff(); // Note: This mode is not fully implemented yet
        }
        else if (switchMode == SwitchMode.ToggleAppear)
        {
            toggleAppearOff(); // Note: This mode is not fully implemented yet
        }
    }


    protected void toggleLinkedBlockShine()
    {
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

    protected void toggleDisappear() // THIS IS NOT THE SAME AS APPEAR, THIS CORRESPONDS TO DISSAPPEAR OFF NOT APPEAR. I REPEAT, THIS CORRESPONDS WITH DISAPPEAR OFF
    {
        //Debug.Log("Toggle Existance start called");
        if (linkedBlock.GetComponent<InteractableBlock>())
        {
            linkedBlock.GetComponent<InteractableBlock>().fullyDisable();
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    protected void toggleDisappearOff()
    {
        //Debug.Log("Toggle Existance reverse called");
        if (linkedBlock.GetComponent<InteractableBlock>())
        {
            linkedBlock.GetComponent<InteractableBlock>().fullyEnable();
            //linkedBlock.GetComponentInChildren<InteractableBlock>().shineBlock(); // Also shine for its light form if it has one
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    protected void toggleAppear()
    {
        if (linkedBlock.GetComponent<SwitchAppearDoor>())
        {
            linkedBlock.GetComponent<SwitchAppearDoor>().appear();
        }
        else
        {
            Debug.LogWarning("Linked block does not have a SwitchAppearDoor component.");
        }
    }

    protected void toggleAppearOff()
    {
        if (linkedBlock.GetComponent<SwitchAppearDoor>())
        {
            linkedBlock.GetComponent<SwitchAppearDoor>().disappear();
        }
        else
        {
            Debug.LogWarning("Linked block does not have a SwitchAppearDoor component.");
        }
    }
    protected void toggleLinkedBlockMovement()
    {
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

    void Start()
    {
        base.init();
        //makeMovable(); // Switches are not movable
    }
    protected override void Update()
    {
        base.Update();
    }

    private void ResetSwitchCD()
    {
        onCD = false;
    }

    private bool checkSwitchCD()
    {
        if (onCD) return false; // If on cooldown, do nothing
        onCD = true;
        Invoke(nameof(ResetSwitchCD), toggleCD); // Set cooldown duration
        return true;
    }
}
