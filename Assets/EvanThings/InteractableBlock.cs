#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using TMPro;


public abstract class InteractableBlock : MonoBehaviour
{
    private Grid _grid;
    [SerializeField] private BlockType lightFormBlock;
    GameObject lightBlock;

    private bool visibleBlock = true; // whether the block is currently visible in the gameWorld
    private bool movableBlock = false; // whether the block can be moved by the player
    private bool isLightForm;
    private Vector3 targetPosition;

    private static float interpolationValue = 0.1f;

    protected void init()
    {
        snapToCellPosition();
        createLightForm();
        targetPosition = transform.position;
    }
    protected virtual void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);
        if (isLightForm)
        {
            if (transform.parent != null)
                transform.position = transform.parent.position;
            else
                Debug.LogWarning("Light form block has no parent to follow.");
        }
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

    /**
     * Moves the block in the given direction if it is movable
     * @param direction The direction to move the block (Vector3.up, Vector3.down, Vector3.left, Vector3.right)
     * @return the new position of the block after moving, or the current position if not movable
     */
    public Vector3 moveBlock(Vector3 direction)
    {
        if (isLightForm) return transform.position; // light forms match the position of the parent block
        Debug.Log("Attempting to move block " + gameObject.name + " in direction " + direction);
        if (!movableBlock) return transform.position;
        
        Vector3Int cellPosition = _grid.WorldToCell(transform.position);

        switch (direction)
        {
            case Vector3 dir when dir == Vector3.up:
                cellPosition += new Vector3Int(0, 1, 0);
                break;
            case Vector3 dir when dir == Vector3.down:
                cellPosition += new Vector3Int(0, -1, 0);
                break;
            case Vector3 dir when dir == Vector3.left:
                cellPosition += new Vector3Int(-1, 0, 0);
                break;
            case Vector3 dir when dir == Vector3.right:
                cellPosition += new Vector3Int(1, 0, 0);
                break;
            default:
                Debug.LogWarning("Invalid direction for moving block.");
                return transform.position;
        }
        targetPosition = _grid.GetCellCenterWorld(cellPosition);
        return cellPosition;
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