using UnityEngine;

/// <summary>
/// For this class, the lightform a door that appears when the switch is activated.
/// /// </summary>
public class SwitchAppearDoor : InteractableBlock
{

    public override void ShineDeinteract()
    {
        //
    }

    public override void ShineInteract()
    {
        //
    }
    void Start()
    {
        base.init();
        onlyInLight = true;
        fullyDisable();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void appear()
    {
        enableBlock(lightBlock);
    }

    public void disappear()
    {
        disableBlock(lightBlock);
    }
}
