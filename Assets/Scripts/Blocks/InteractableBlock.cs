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
using NUnit.Framework;
using System.Collections.Generic;


public abstract class InteractableBlock : MonoBehaviour
{
    protected Grid _grid;
    [SerializeField] private BlockType lightFormBlock;
    protected GameObject lightBlock;
    [SerializeField] protected bool movableBlock = false; // whether the block can be moved by the player
    [SerializeField] protected bool onlyInLight = false; // whether the block only appears in the light world
    [SerializeField] protected bool isToggleMoveLinkedBlock = false; // Important so toggle Move Blocks Die when they are placed in another block
    protected bool isLightForm = false;
    protected Vector3 targetPosition;
    protected SpriteRenderer spriteRenderer;

    private static float interpolationValue = 0.1f;

    public int shineFrames = 0; // counts how many frames the block has been lit
    public int currentShineFrames = 0; // last known amount of shineframes

    [Header("For Debugging")]
    [SerializeField] protected bool visibleBlock = true; // whether the block is currently visible in the gameWorld
    [SerializeField] private bool isShining = false; // so you dont reshine it when ur spamming raycasts at it
    [SerializeField] protected bool fullDisabled = false; // So it doesn't start shining again events (like a switch opening a door)

    [SerializeField] public static List<Vector3Int> movingBlockCordinates = new List<Vector3Int>(); // IN CASE WE DECIDE TO CHANGE HOW ENEMIES WORK

    protected void init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isLightForm) return; // light forms don't need to init
        snapToCellPosition();
        createLightForm();
        targetPosition = transform.position;
        if (onlyInLight)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            changeVisibility(false);
        }
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        checkDeath(); // only destroyable if the block is movable
        checkShineReset();
        interpolateMovementForBlock();
    }

    /**
    * Removes the collider and sprite render, Invisible and No Collision, but script is still active 
    */
    public static void disableBlock(GameObject block)
    {
        if (block.GetComponent<InteractableBlock>().isOnlyInLight())
        {
            //Debug.Log("Block has no DarkMode cannot disable: " + block.name);
            return;
        }
        block.GetComponent<SpriteRenderer>().enabled = false;
        //block.GetComponent<BoxCollider2D>().enabled = false;
        block.GetComponent<InteractableBlock>().changeVisibility(false);
    }
    /**
    * Adds the collider and sprite render. Visible and Solid
    */
    public static void enableBlock(GameObject block)
    {
        if (block.GetComponent<InteractableBlock>().isOnlyInLight())
        {
            //Debug.Log("Block has no DarkMode cannot enable: " + block.name);
            return;
        }
        block.GetComponent<SpriteRenderer>().enabled = true;
        //block.GetComponent<BoxCollider2D>().enabled = true;
        block.GetComponent<InteractableBlock>().changeVisibility(true);
    }

    /**
     * Static Method to check if a ray hit an interactable block, and if so, call its addShineCounter method and start shining
     * Works for RayCastHit2D
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
     * Static Method to check if a ray hit an interactable block, and if so, call its addShineCounter method and start shining
     * Works for colliders
     */
    public static void checkRayCollision(Collider2D hit)
    {
        //Debug.Log("Ray hit: " + hit.collider.name);
        if (hit != null && hit.gameObject.layer == LayerMask.NameToLayer("Blocks"))
        {
            InteractableBlock block = hit.gameObject.GetComponent<InteractableBlock>();
            if (block != null && !block.checkIsLightForm())
            {
                block.addShineCounter();
                block.startShining();
            }
        }
    }

    public bool isOnlyInLight()
    {
        return onlyInLight;
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
    * Event Called by player to interact with block 
    */
    public bool plrInteractEvent(Vector3Int dir)
    {
        if (movableBlock)
        {
            moveBlock(dir);
            return true;
        }
        else
        {
            // implement other interactions here (dialogue, other stuff, etc)
            return false;
        }
    }
    /**
     * Event for enemies interacting with blocks, called by enemy. Different from Player because, enemies won't use other features like
     * dialogue, and anything else we decide to do
     */
    public bool enemyInteractEvent(Vector3Int dir)
    {
        if (movableBlock)
        {
            moveBlock(dir);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool canMove(Vector3Int direction)
    {
        Vector3Int cellPosition = _grid.WorldToCell(transform.position);
        if (isLightForm || !movableBlock) return false; // light forms match the position of the parent block
        //Debug.Log(direction);
        if (direction != Vector3Int.up && direction != Vector3Int.down && direction != Vector3Int.left && direction != Vector3Int.right)
        {
            Debug.LogWarning("Invalid direction for moving block.");
            return false;
        }
        //ADD WALL CHECK
        if (wallCheck(cellPosition + direction))
        {
            return false;
        }

        return true;
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
        SoundManager.Instance.PlayAudio(1, AudioSourceType.Push);
        Vector3Int cellPosition = _grid.WorldToCell(transform.position);
        if (!canMove(direction)) { return cellPosition; }

        cellPosition += direction;
        targetPosition = _grid.GetCellCenterWorld(cellPosition);
        movingBlockCordinates.Add(cellPosition);
        Invoke("clearMovingBlockCoords", PlayerMove.cdDuration); // clear the used coords once done
        return cellPosition;
    }

    /**
     * Turns the block to its light form
     */
    public virtual void shineBlock()
    {
        if (fullDisabled)
        {
             //Debug.Log("Block is fully disabled, cannot shine: " + gameObject.name);
            return;
        }
        if (isLightForm || lightFormBlock == BlockType.repeat) return;
        disableBlock(gameObject);
        if (lightBlock != null) // if not EmptySpace
            enableBlock(lightBlock);
    }
    /**
     * Turns the block back to its shadow form
     */
    public virtual void deshineBlock()
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
    /**
     * Makes a Block Toggle Move Swtich linked Block
     */
    public void makeToggleMoveBlock()
    {
        isToggleMoveLinkedBlock = true;
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
        
        lightBlock.name = lightFormBlock+ "_LightForm";
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
            //Debug.Log("b");
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
    public bool wallCheck(Vector3Int cellPos)
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(_grid.GetCellCenterWorld(cellPos), new Vector2(0.1f, 0.1f), 0);
        //GameObject wall = tilemap.GetInstantiatedObject(cellPos);
        foreach (Collider2D collider in colliderList)
        {
            //Debug.Log(collider.gameObject.layer + " | " + LayerMask.LayerToName(collider.gameObject.layer));
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("interactableBlock Not Found, for block movement"); }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible() || collider.GetComponent<Enemy>())
            {

                return true;
            }
            // CHECK IF BLOCK ONLY DOOR
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable is NoBlockDoor)
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
        if (!movableBlock && !isToggleMoveLinkedBlock) { return; }
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.1f, 0.1f), 0);
        foreach (Collider2D collider in colliderList)
        {
            if (collider.gameObject == gameObject) continue; // skip self
            
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                Debug.Log("Block Destroyed");
                Destroy(gameObject);
            }
        }
    }

    
    private void clearMovingBlockCoords()
    {
        movingBlockCordinates.Clear();
    }
    
}


/*
 * Updated 10/18/2025 4:50 AM
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
 *  - both "light and shadow" forms of a block will have the same movable state.
 *  
 *  BLOCK TYPES:
 *  -> HARDWALL = solid wall, light never passes
 *  -> ShadowWall = General Wall, by default will disappear in light
 *  -> Switch = Reacts to light, needs a toggleBlock
 *  -> PressurePlate = Switch that reacts to Players, Enemies, Blocks. CAN BE SET TO ONLY WORK WITH CERTAIN GAMEOBJECTS
 *  -> Mirror: Can be moved
 *  -> Dark Mirror: Cant be moved
 *  -> SwitchAppearDoor: MUST be paired with the switch when using toggle appear
 *  -> SwitchDisappearDoor: Can be paired with Switch when using toggleDisappear, not nessecary though
 *  
 *  SWITCHES:
 *  - If you are using a Toggle Move Switch, you should set isToggleMoveLinkedBlock to true (But the script should already do this for you)
 *  - All Switches should start with a link block
 *  - Switches should always be set to "repeat" because they need to be interactable in both worlds.
 * 
 *  Block Death:
 *  - Blocks will only die if they are either movable or a Toggle Movement Block
 *  
 *  FlashLightNote:
 *  - Whenever a Raycasthits an interactableBlock, it creates a little cube so that when blocks are ontop of eachother 
 * 
 * Method Stuff:
 * - visibleBlock: whether the block is currently visible in the game world
 * - movableBlock: whether the block can be moved by the player
 * - isLightForm: whether the block is a light form (follows parent position, no collider)
 * - lightFormBlock: the type of block this block turns into when it shines (set to emptySpace for no light form)
 * - makeLightBlock(): called on a light block to make it know it's a light block, Basically only used by this script
 * - shine() and deshine() are the methods to call to change the block's form, they turn invisible, but their colliders + 
 *  scripts are still active <-- (must account for this).
 *  - Anything that has clearMovingBlockCoords()
 * 
 * 
 * 
 * 
 */