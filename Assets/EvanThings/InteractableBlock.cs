#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


public abstract class InteractableBlock : MonoBehaviour
{
    private Grid _grid;
    [SerializeField] private BlockType lightFormBlock;
    private bool isLightForm;
    GameObject lightBlock;

    private bool visibleBlock = true; // whether the block is currently visible in the gameWorld
    private bool movableBlock = false; // whether the block can be moved by the player

    protected void init()
    {
        snapToCellPosition();
        createLightForm();
    }
    /**
    * Called on a light block to make it know it's a light block, Basically only used by this script
    */
    public void makeLightBlock()
    {
        isLightForm = true;
    }

    /**
    * Abstract method to define interaction behavior. 
    */
    public abstract void Interact();

    // Public Methods
    /**
     * Turns on the block's collider, making it solid
     */
    public void makeSolid() 
    {
        var collider = gameObject.GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }
    /**
     * Turns off the block's collider, making it non-solid
     */
    public void makeUnsolid()
    {
        var collider = gameObject.GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }
    /**
     * Makes block movable by the player, changes bool to true
     */
    public void makeMovable()
    {
        movableBlock = true;
    }
    /**
     * Makes block immovable by the player, changes bool to false
     */
    public void makeImmovable()
    {
        movableBlock = false;
    }

    public Vector2 moveBlock()
    {
        return transform.position;
    }

    /**
     * Turns the block to its light form
     */
    public void shineBlock()
    {
        if (isLightForm || lightFormBlock == BlockType.repeat) return;
        disableBlock(gameObject);
        if (lightBlock != null) // if not EmptySpace
            enableBlock(lightBlock);
    }
    /**
     * Turns the block back to its shadow form
     */
    public void deshineBlock()
    {
        if (isLightForm || lightFormBlock == BlockType.repeat) return;
        enableBlock(gameObject);
        if (lightBlock != null) // if not EmptySpace
            disableBlock(lightBlock);
    }

    // private methods

    /**
     * changes the current block to its light form
     * @return GameObject of the light form created
     */
    private void createLightForm()
    {
        lightBlock = null;
        if (isLightForm || lightFormBlock == BlockType.emptySpace || lightFormBlock == BlockType.repeat) return; // DONT MAKE IT IF EMPTY OR REPEAT
        Debug.Log("Creating light form for " + gameObject.name);
        switch (lightFormBlock)
        {
            case BlockType.ShadowWall:
                // Create light form for Shadowwall with script inside of it
                lightBlock = Instantiate(BlockManager.instance.getShadowWallPrefab(), transform.position, Quaternion.identity, transform.parent);
                lightBlock.GetComponent<ShadowWall>().makeLightBlock();
                break;
            default:
                Debug.LogWarning("Light form not defined for this block type.");
                break;
        }
        lightBlock.transform.SetParent(transform);
        
        lightBlock.name = gameObject.name + "_LightForm";
        disableBlock(lightBlock);
    }
    /**
     * On start, snaps the object to the nearest cell position in the grid. Basically, if you aren't too precise on ur placement, it will fix it for you.
     */
    private void snapToCellPosition()
    {
        _grid = GetComponentInParent<Grid>();
        if (_grid == null)
        {
            Debug.LogError("Grid component not found in parent hierarchy.");
            return;
        }
        Vector3Int cellPosition = _grid.WorldToCell(transform.position);

        //Debug.Log($"Cell Position: {cellPosition}");
        transform.position = _grid.GetCellCenterWorld(cellPosition);
    }
    /**
    * Removes the collider and sprite render, Invisible and No Collision, but script is still active 
    */
    private void disableBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().enabled = false;
        block.GetComponent<BoxCollider2D>().enabled = false;
        visibleBlock = false;
    }
    /**
    * Adds the collider and sprite render. Visible and Solid
    */
    private void enableBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().enabled = true;
        block.GetComponent<BoxCollider2D>().enabled = true;
        visibleBlock = true;
    }
    /**
     * Returns whether the block is currently visible in the game world
     */
    public bool isVisible()
    {
        return visibleBlock;
    }
}