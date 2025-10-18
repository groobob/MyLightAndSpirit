using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : InteractableBlock
{
    [Header("Switch Mode & Linked Block")]
    [SerializeField] private GameObject linkedBlock;
    [SerializeField] SwitchMode switchMode;
    private enum SwitchMode
    {
        toggleShine,
        toggleMovement,
        toggleDisappear,
        toggleAppear
    }

    [Header("!!!!Switches should be set to Repeat!!!!")] 
    
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private bool onCD = false; // To prevent rapid toggling 
    [SerializeField] private float toggleCD = 0.5f; // Cooldown duration in seconds

    public override void ShineInteract()
    {
        if (!checkSwitchCD()) return;
        spriteRenderer.sprite = onSprite;
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
            toggleDisappear(); // Note: This mode is not fully implemented yet
        }
        else if (switchMode == SwitchMode.toggleAppear)
        {
            toggleAppear(); // Note: This mode is not fully implemented yet
        }
    }

    public override void ShineDeinteract()
    {
        spriteRenderer.sprite = offSprite;
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
            toggleDisappearOff(); // Note: This mode is not fully implemented yet
        }
        else if (switchMode == SwitchMode.toggleAppear)
        {
            toggleAppearOff(); // Note: This mode is not fully implemented yet
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

    private void toggleDisappear() // THIS IS NOT THE SAME AS APPEAR, THIS CORRESPONDS TO DISSAPPEAR OFF NOT APPEAR. I REPEAT, THIS CORRESPONDS WITH DISAPPEAR OFF
    {
        //Debug.Log("Toggle Existance start called");
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<InteractableBlock>())
        {
            linkedBlock.GetComponent<InteractableBlock>().fullyDisable();
        }
        else
        {
            Debug.LogWarning("Linked block does not have an InteractableBlock component.");
        }
    }

    private void toggleDisappearOff()
    {
        //Debug.Log("Toggle Existance reverse called");
        if (!visibleBlock) return; // Do nothing if the switch is invisible
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

    private void toggleAppear()
    {
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<SwitchAppearDoor>())
        {
            linkedBlock.GetComponent<SwitchAppearDoor>().appear();
        }
        else
        {
            Debug.LogWarning("Linked block does not have a SwitchAppearDoor component.");
        }
    }

    private void toggleAppearOff()
    {
        if (!visibleBlock) return; // Do nothing if the switch is invisible
        if (linkedBlock.GetComponent<SwitchAppearDoor>())
        {
            linkedBlock.GetComponent<SwitchAppearDoor>().disappear();
        }
        else
        {
            Debug.LogWarning("Linked block does not have a SwitchAppearDoor component.");
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
