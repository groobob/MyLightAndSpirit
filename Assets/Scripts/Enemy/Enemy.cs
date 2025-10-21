using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class Enemy : MonoBehaviour
{
    public enum InitalDirection
    {
        Up, Down, Left, Right
    }

    [SerializeField] protected InitalDirection initalDirection = InitalDirection.Right;
    protected Grid _grid;
    [SerializeField] float interpolationValue = 0.35f;
    protected Vector3Int direction;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] private string environmentLayer = "Blocks";

    protected Vector3 targetPosition;
    protected void init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (initalDirection)
        {
            case InitalDirection.Left:   direction = Vector3Int.left; break;
            case InitalDirection.Right: direction = Vector3Int.right; break;
            case InitalDirection.Up: direction = Vector3Int.up; break;
            case InitalDirection.Down: direction = Vector3Int.down; break;
            default: direction = Vector3Int.right; break;
        }
        _grid = GetComponentInParent<Grid>();
        snapToCellPosition();
        targetPosition = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        //Debug.DrawLine(transform.position, new Vector3(direction.x, direction.y, 0) * 0.5f, Color.red, 0.5f); // Mark hit point
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);
    }
    /**
     * Moves all enemies in the given level object by calling their movement method.
     */
    public static void moveAllEnemies(GameObject levelObject, Vector2Int playerDirection)
    {
        Enemy[] enemies = levelObject.GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy is CopyEnemy)
            {
                CopyEnemy enemy1 = enemy as CopyEnemy;
                enemy1.giveDirection(playerDirection);
            }
            enemy.movement();
        }
    }

    /**
     * Abstract method to define movement behavior. Called by moveAllEnemies.
     */
    protected abstract void movement();

    /*
     * Checks if the enemy hits a wall or player in front of it. If it hits a wall, it reverses direction. If it hits a player, it logs a message (to be replaced with player damage).
     * @return true if a player is hit OR if the enemy is cornered, false otherwise. If true is returned, movement is stopped.
     */
    protected virtual bool hitCheck()
    {
        //PlayerRay
        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + direction, new Vector3(direction.x, direction.y, 0), 1f);
        // Implement Player Checking Here
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(transform.position + direction, new Vector2(0.1f, 0.1f), 0);

        bool playerFound = false;
        bool enemyCornered = false;
        bool pushedBlock = false;
        bool coordinateUsed = false;

        foreach (Collider2D hit in colliderList)
        {
            

            InteractableBlock interactable = hit.gameObject.GetComponent<InteractableBlock>();
            if (hit != null && hit.CompareTag("Player"))
            {
                PlayerMove plrMove = hit.GetComponent<PlayerMove>();
                if (plrMove != null)
                {
                    //plrMove.killPlayer();
                }
                //playerFound = true;
            }
            else if (hit != null && hit.gameObject.layer == LayerMask.NameToLayer(environmentLayer) && interactable.isVisible() && interactable.canMove(direction))
            {
                enemyMoveBlock(interactable.gameObject);
                pushedBlock = true;
            }
            else if (hit != null && hit.gameObject.layer == LayerMask.NameToLayer(environmentLayer) && interactable.isVisible())
            {
                //Debug.Log("Enemy hit a wall! Reversing direction.");
                direction = -direction; // Reverse direction on wall hit
                //RaycastHit2D[] hit2D = Physics2D.RaycastAll(transform.position, new Vector3(direction.x, direction.y, 0), 1f);
                Collider2D[] corneredCollider = Physics2D.OverlapBoxAll(transform.position + direction, new Vector2(0.1f, 0.1f), 0);
                foreach (Collider2D collider in corneredCollider)
                {
                    InteractableBlock cornerInteractable = collider.gameObject.GetComponent<InteractableBlock>();
                    if (collider != null && collider.gameObject.layer == LayerMask.NameToLayer(environmentLayer) && cornerInteractable.isVisible())
                    {
                        //Debug.Log(collider.gameObject);
                        enemyCornered = true; // Enemy is cornered, cannot move
                    }
                }
            }
            
            
        }
        /*
        bool inABlock = InteractableBlock.movingBlockCordinates.Contains(_grid.WorldToCell(transform.position + direction));
        if (inABlock) // if current block is occupied
        {
            Debug.Log("Block is occupied");
            coordinateUsed = true;
        }
        */

        return playerFound || enemyCornered || pushedBlock || coordinateUsed;
    }


    private void enemyMoveBlock(GameObject block)
    {
        Vector3Int blockMovePos = new Vector3Int(direction.x, direction.y, 0);
        InteractableBlock interactable = block.gameObject.GetComponent<InteractableBlock>();
        if (interactable != null)
        {
            interactable.enemyInteractEvent(blockMovePos);
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
}
