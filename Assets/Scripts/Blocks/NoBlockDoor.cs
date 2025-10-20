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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.Update();
    }
}
