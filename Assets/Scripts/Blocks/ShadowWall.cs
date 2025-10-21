using UnityEngine;

public class ShadowWall : InteractableBlock
{
    void Start()
    {
        base.init();
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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
