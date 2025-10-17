using UnityEngine;

public class Movable : InteractableBlock
{

    public override void ShineDeinteract()
    {
        //
    }

    public override void ShineInteract()
    {
        //
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.init();
        makeMovable();
    }

    protected override void Update()
    {
        base.Update();
    }
}
