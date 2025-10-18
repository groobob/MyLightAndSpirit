using UnityEngine;

public class NoBlockDoor : InteractableBlock
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
        changeVisibility(false);
    }

    protected override void Update()
    {
        base.Update();
    }
}
