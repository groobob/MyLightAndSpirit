using UnityEngine;
using static UnityEngine.UI.Image;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected Vector3Int initalDirection = Vector3Int.right;
    protected Grid _grid;
    [SerializeField] float interpolationValue = 0.1f;
    protected Vector3Int direction;
    protected SpriteRenderer spriteRenderer;

    private string environmentLayer = "Blocks";

    protected Vector3 targetPosition;
    protected void init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = initalDirection;
        _grid = GetComponentInParent<Grid>();
        snapToCellPosition();
        targetPosition = transform.position;
    }

    protected virtual void Update()
    {
        //Debug.DrawLine(transform.position, new Vector3(direction.x, direction.y, 0) * 0.5f, Color.red, 0.5f); // Mark hit point
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);
    }
    /**
     * Moves all enemies in the given level object by calling their movement method.
     */
    public static void moveAllEnemies(GameObject levelObject)
    {
        Enemy[] enemies = levelObject.GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemies)
        {
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, new Vector3(direction.x, direction.y, 0), 1f);
        // Implement Player Checking Here

        bool playerFound = false;
        bool enemyCornered = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                //Debug.Log("Enemy hit the player!"); //Implement player damage here
                playerFound = true;
            }
            else if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer(environmentLayer))
            {
                //Debug.Log("Enemy hit a wall! Reversing direction.");
                direction = -direction; // Reverse direction on wall hit
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, new Vector3(direction.x, direction.y, 0), 1f);
                if (hit2D.collider != null && hit2D.collider.gameObject.layer == LayerMask.NameToLayer(environmentLayer))
                {
                    enemyCornered = true; // Enemy is cornered, cannot move
                }
            }
        }

        return playerFound || enemyCornered;
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
