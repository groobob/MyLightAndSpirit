using UnityEngine;

public class ShadowWall : InteractableBlock
{
    void Start()
    {
        base.init();
    }
    protected override void Update()
    {
        base.Update();
        //testingCode()
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


    private void testingCode()
    {
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
            moveBlock(Vector3Int.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveBlock(Vector3Int.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveBlock(Vector3Int.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveBlock(Vector3Int.right);
        }
    }

}
