using UnityEngine;

public class ShadowWall : InteractableBlock
{
    void Start()
    {
        base.init();
        base.Update();
        makeMovable();
    }
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveBlock(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveBlock(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveBlock(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveBlock(Vector3.right);
        }
    }

    public override void Interact()
    {
         //Do nothing, it's a wall
    }
}
