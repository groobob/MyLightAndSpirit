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
    private GameObject lightBlock;
    [SerializeField] protected bool movableBlock = false; // whether the block can be moved by the player
    protected bool isLightForm = false;
    protected Vector3 targetPosition;

    private static float interpolationValue = 0.1f;

    public int shineFrames = 0; // counts how many frames the block has been lit
    public int currentShineFrames = 0; // last known amount of shineframes

    [Header("For Debugging")]
    [SerializeField] protected bool visibleBlock = true; // whether the block is currently visible in the gameWorld
    [SerializeField] private bool isShining = false;
    [SerializeField] private bool fullDisabled = false;

    protected void init()
    {
        if (isLightForm) return; // light forms don't need to init
        snapToCellPosition();
        createLightForm();
        targetPosition = transform.position;
    }
    protected virtual void Update()
    {
        if (movableBlock) { checkDeath(); } // only destroyable if the block is movable
        checkShineReset();
        interpolateMovementForBlock();
    }

    /**
    * Removes the collider and sprite render, Invisible and No Collision, but script is still active 
    */
    public static void disableBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().enabled = false;
        //block.GetComponent<BoxCollider2D>().enabled = false;
        block.GetComponent<InteractableBlock>().changeVisibility(false);
    }
    /**
    * Adds the collider and sprite render. Visible and Solid
    */
    public static void enableBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().enabled = true;
        //block.GetComponent<BoxCollider2D>().enabled = true;
        block.GetComponent<InteractableBlock>().changeVisibility(true);
    }

    /**
     * Static Method to check if a ray hit an interactable block, and if so, call its addShineCounter method
     */
    public static void checkRayCollision(RaycastHit2D hit)
    {
        //Debug.Log("Ray hit: " + hit.collider.name);
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Blocks"))
        {
            InteractableBlock block = hit.collider.gameObject.GetComponent<InteractableBlock>();
            if (block != null && !block.checkIsLightForm())
            { 
                block.addShineCounter();
                block.startShining();
            }
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
    public abstract void ShineInteract();

    /**
    * Abstract method to define deinteraction behavior. 
    */
    public abstract void ShineDeinteract();
    /**
    * Abstract method to define playerInteraction behavior. 
    */
    public void plrInteractEvent(Vector3Int dir)
    {
        if (movableBlock)
        {
            moveBlock(dir);
        }
        else
        {
            // implement other interactions here (dialogue, other stuff, etc)
        }
    }

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
     * Checks if the block is a light form
     * @return true if the block is a light form, false otherwise
     */
    public bool checkIsLightForm()
    {
        return isLightForm;
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
     * @return the new cell position of the block after moving, or the current position if not movable
     */
    public Vector3Int moveBlock(Vector3Int direction)
    {
        Vector3Int cellPosition = _grid.WorldToCell(transform.position);
        if (isLightForm || !movableBlock) return cellPosition; // light forms match the position of the parent block
        //Debug.Log("Attempting to move block " + gameObject.name + " in direction " + direction);
        Debug.Log(direction);
        if (direction != Vector3Int.up && direction != Vector3Int.down && direction != Vector3Int.left && direction != Vector3Int.right)
        {
            Debug.LogWarning("Invalid direction for moving block.");
            return cellPosition;
        }
        //ADD WALL CHECK
        if (wallCheck(cellPosition + direction))
        {
            return cellPosition;
        }

        cellPosition += direction;
        targetPosition = _grid.GetCellCenterWorld(cellPosition);
        return cellPosition;
    }

    /**
     * Turns the block to its light form
     */
    public void shineBlock()
    {
        if (fullDisabled)
        {
             //Debug.Log("Block is fully disabled, cannot shine: " + gameObject.name);
            return;
        }
        if (isLightForm || lightFormBlock == BlockType.repeat) return;
        //Debug.Log("Shining block: " + gameObject.name);
        disableBlock(gameObject);
        if (lightBlock != null) // if not EmptySpace
            enableBlock(lightBlock);
    }
    /**
     * Turns the block back to its shadow form
     */
    public void deshineBlock()
    {
        if (fullDisabled)
        {
            //Debug.Log("Block is fully disabled, cannot deshine: " + gameObject.name);
            return;
        }
        if (isLightForm || lightFormBlock == BlockType.repeat) return;
        //Debug.Log("Deshining block: " + gameObject.name);
        enableBlock(gameObject);
        if (lightBlock != null) // if not EmptySpace
            disableBlock(lightBlock);
    }

    public void changeVisibility(bool isVisible)
    {
        visibleBlock = isVisible;
    }
    /**
     * Disables both the block and its light form, making them invisible and non-collidable
     */
    public void fullyDisable()
    {
        //Debug.Log("Fully disabling block: " + gameObject.name);
        disableBlock(gameObject);
        if (lightBlock != null)
            disableBlock(lightBlock);
        changeVisibility(false);
        fullDisabled = true;
    }

    public void fullyEnable()
    {
        //Debug.Log("Fully enabling block: " + gameObject.name);
        enableBlock(gameObject);
        changeVisibility(true);
        fullDisabled = false;
        isShining = false;
    }
    public bool isFullDisabled()
    {
        return fullDisabled;
    }
    /**
     * Returns whether the block can be moved by the player
     */
    public bool isMovable()
    {
        return movableBlock;
    }
    
    /**
     * Returns whether the block is currently visible in the game world
     */
    public bool isVisible()
    {
        return visibleBlock;
    }


    /**
     * Increments the shine counter
     */
    public void addShineCounter()
    {
        shineFrames += 1;
    }

    private void interpolateMovementForBlock()
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
     * changes the current block to its light form
     * @return GameObject of the light form created
     */
    private void createLightForm()
    {
        lightBlock = null;
        if (isLightForm || lightFormBlock == BlockType.emptySpace || lightFormBlock == BlockType.repeat) return; // DONT MAKE IT IF EMPTY OR REPEAT
        //Debug.Log("Creating light form for " + gameObject.name);
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
    private void startShining() 
    {
        if (!isShining && !isLightForm && !fullDisabled)
        {
            isShining = true;
            shineBlock();
            //Debug.Log("Block started shining");
            ShineInteract();
        }
    }

    private void checkShineReset()
    {
        if (Mathf.Abs(shineFrames - currentShineFrames) == 0 && isShining)
        {
            //Debug.Log("a");
            shineFrames = 0;
            currentShineFrames = 0;
            // Debug.Log("Shine frames reset for block: " + gameObject.name);
            isShining = false;
            deshineBlock();
            ShineDeinteract(); // Consider adding a deinteract method if needed?
        }
        currentShineFrames = shineFrames;
    }
    

    /**
     * On start, snaps the object to the nearest cell position in the grid. Basically, if you aren't too precise on ur placement, it will fix it for you.
     */
    private void snapToCellPosition()
    {
        _grid = transform.parent.GetComponentInParent<Grid>();
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
     * Checks if the block is trying to move into a wall
     * @return true if there is a wall, false otherwise
     */
    private bool wallCheck(Vector3Int cellPos)
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(_grid.GetCellCenterWorld(cellPos), new Vector2(0.1f, 0.1f), 0);
        //GameObject wall = tilemap.GetInstantiatedObject(cellPos);
        foreach (Collider2D collider in colliderList)
        {
            //Debug.Log(collider.gameObject.layer + " | " + LayerMask.LayerToName(collider.gameObject.layer));
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("interactableBlock Not Found, for block movement"); }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {

                return true;
            }
        }
        return false;
    }

    /**
     * Checks if the block is touching another block
     * If so, restarts the level
     */
    private void checkDeath()
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.1f, 0.1f), 0);
        foreach (Collider2D collider in colliderList)
        {
            if (collider.gameObject == gameObject) continue; // skip self

            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("interactableBlock Not Found"); }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                Debug.Log("Block Destroyed");
                Destroy(gameObject);
            }
        }
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