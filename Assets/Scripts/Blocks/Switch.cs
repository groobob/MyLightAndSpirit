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

    protected enum SwitchState
    {
        On,
        Off,
    }

    [Header("!!!!Switches should be set to Repeat!!!!")] 
    
    [SerializeField] protected Sprite onSprite;
    [SerializeField] protected Sprite offSprite;
    protected bool onCD = false; // To prevent rapid toggling 
    protected float toggleCD = 0.1f; // Cooldown duration in seconds

    public override void ShineInteract()
    {
        if (!checkSwitchCD()) return;
        spriteRenderer.sprite = onSprite;
        SwitchState changeState = SwitchState.On;
        if (linkedBlock == null)
        {
            Debug.Log("interactable Block has been destroyed");
            return;
        }
        if (switchMode == SwitchMode.ToggleShine)
        {
            toggleLinkedBlockShine(changeState);
        }
        else if (switchMode == SwitchMode.ToggleMovement)
        {
            toggleLinkedBlockMovement(changeState);
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
        SwitchState changeState = SwitchState.Off;
        if (linkedBlock == null)
        {
            Debug.Log("interactable Block has been destroyed");
            return;
        }
        if (switchMode == SwitchMode.ToggleShine)
        {
            toggleLinkedBlockShine(changeState);
        }
        else if (switchMode == SwitchMode.ToggleMovement)
        {
            toggleLinkedBlockMovement(changeState);
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


    protected void toggleLinkedBlockShine(SwitchState changeState)
    {
        if (linkedBlock.GetComponent<InteractableBlock>() && changeState == SwitchState.On)
        {
            linkedBlock.GetComponent<InteractableBlock>().shineBlock();
            
        }
        else if (linkedBlock.GetComponent<InteractableBlock>() && changeState == SwitchState.Off)
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
        if (linkedBlock.GetComponent<SwitchDisappearDoor>())
        {
            linkedBlock.GetComponent<SwitchDisappearDoor>().disappear();
        }
        else if (linkedBlock.GetComponent<InteractableBlock>())
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
        if (linkedBlock.GetComponent<SwitchDisappearDoor>())
        {
            linkedBlock.GetComponent<SwitchDisappearDoor>().appear();
        }
        else if (linkedBlock.GetComponent<InteractableBlock>())
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
    protected void toggleLinkedBlockMovement(SwitchState changeState)
    {
        if (linkedBlock.GetComponent<InteractableBlock>() && changeState == SwitchState.Off)
        {
            linkedBlock.GetComponent<InteractableBlock>().makeImmovable();
            //linkedBlock.GetComponentInChildren<InteractableBlock>().makeImmovable(); // Also make immovable for its light form if it has one
        }
        else if (linkedBlock.GetComponent<InteractableBlock>() && changeState == SwitchState.On)
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
        if (switchMode == SwitchMode.ToggleMovement)
        {
            linkedBlock.GetComponent<InteractableBlock>().makeToggleMoveBlock();
        }
        //makeMovable(); // Switches are not movable
    }

    protected override void Awake()
    {
        base.Awake();
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
