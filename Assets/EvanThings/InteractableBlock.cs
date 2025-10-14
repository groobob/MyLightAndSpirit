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

    protected bool visibleBlock = true; // whether the block is currently visible in the gameWorld
    [SerializeField] protected bool movableBlock = false; // whether the block can be moved by the player
    protected bool isLightForm = false;
    protected Vector3 targetPosition;

    private static float interpolationValue = 0.1f;

    protected void init()
    {
        if (isLightForm) return; // light forms don't need to init
        Debug.Log("Initializing block: " + gameObject.name);
        snapToCellPosition();
        createLightForm();
        targetPosition = transform.position;
    }
    protected virtual void Update()
    {
        if (transform.position != targetPosition)
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
        //Debug.Log("Making block immovable: " + gameObject.name);
        movableBlock = false;
    }
    /**
     * Checks if the block is visible in the game world
     * @return true if the block is visible, false otherwise
     */
    public bool isVisibleBlock()
    {
        return visibleBlock;
    }

    /**
     * Moves the block in the given direction if it is movable
     * @param direction The direction to move the block (Vector3.up, Vector3.down, Vector3.left, Vector3.right)
     * @return the new position of the block after moving, or the current position if not movable
     */
    public Vector3 moveBlock(Vector3 direction)
    {
        if (isLightForm) return transform.position; // light forms match the position of the parent block
        //Debug.Log("Attempting to move block " + gameObject.name + " in direction " + direction);
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

    public void changeVisibilityTag(bool isVisible)
    {
        visibleBlock = isVisible;
    }

    public bool isMovable()
    {
        return movableBlock;
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
                break;
            case BlockType.Switch:
                lightBlock = Instantiate(BlockManager.instance.getSwitchPrefab(), transform.position, Quaternion.identity, transform.parent);
                break;
            default:
                Debug.LogWarning("Light form not defined for this block type.");
                break;
        }
        lightBlock.transform.SetParent(transform);
        
        lightBlock.name = gameObject.name + "_LightForm";
        InteractableBlock blockScript = lightBlock.GetComponent<InteractableBlock>();
        if (blockScript == null)
        {
            Debug.LogError("Light form prefab does not have an InteractableBlock script attached.");
            return;
        }
        else
        {
            blockScript.makeLightBlock();
            if (movableBlock)
                blockScript.makeMovable();
            else
                blockScript.makeImmovable();
            disableBlock(lightBlock);
        }
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
        block.GetComponent<InteractableBlock>().changeVisibilityTag(false);
    }
    /**
    * Adds the collider and sprite render. Visible and Solid
    */
    private void enableBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().enabled = true;
        block.GetComponent<BoxCollider2D>().enabled = true;
        block.GetComponent<InteractableBlock>().changeVisibilityTag(true);
    }
    /**
     * Returns whether the block is currently visible in the game world
     */
    public bool isVisible()
    {
        return visibleBlock;
    }
}


/*
 * Updated 10/14/2025 5:30 AM
 * 
 * Hi this is a user tutorial lmao!!!
 * 
 * So this is the base class for all interactable blocks in the game. 
 * 
 * Pretty much you can just drag around the prefabs to build the levels but here is stuff you might want to know:
 * 
 *  - the "light form block" serialized field is the type of block this block turns into when it shines. 
 *  - if you want a block to not have a light form, set it to emptySpace   
 *  - if you want a block to be a repeat block (The same blocktype in both dark and light worlds), set it to repeat
 *  - Feel free to change "moveAble" blocks in the inspector to make them movable or not (Note that some methods might override this)
 *  - Switches should always be set to "repeat" because they need to be interactable in both worlds.
 *  - both "light and shadow" forms of a block will have the same movable state.
 *  
 *  - shine() and deshine() are the methods to call to change the block's form, they turn invisible + colliders turned off, but their scripts are still active <-- (must account for this).
 * 
 * 
 * Method Stuff:
 * - visibleBlock: whether the block is currently visible in the game world
 * - movableBlock: whether the block can be moved by the player
 * - isLightForm: whether the block is a light form (follows parent position, no collider)
 * - lightFormBlock: the type of block this block turns into when it shines (set to emptySpace for no light form)
 * - makeLightBlock(): called on a light block to make it know it's a light block, Basically only used by this script
 * 
 * 
 * 
 * 
 */