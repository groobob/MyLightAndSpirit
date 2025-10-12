using UnityEngine;

public class ShadowWall : InteractableBlock
{
    void Start()
    {
        base.init();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {   
            shineBlock();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            deshineBlock();
        }
    }

    public override void Interact()
    {
         //Do nothing, it's a wall
    }
}
