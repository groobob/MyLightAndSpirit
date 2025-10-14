using UnityEngine;
using static UnityEngine.UI.Image;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Vector3Int direction = Vector3Int.right;
    private Grid _grid;
    [SerializeField] float interpolationValue = 0.1f;

    private Vector3 targetPosition;
    void Start()
    {
        _grid = GetComponentInParent<Grid>();
        snapToCellPosition();
        targetPosition = transform.position;
    }
    void Update()
    {
        //Debug.DrawLine(transform.position, new Vector3(direction.x, direction.y, 0) * 0.5f, Color.red, 0.5f); // Mark hit point
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);

        if (Input.GetKeyDown(KeyCode.M))
        {
            movement();
        }
    }

    private void movement()
    {
        if (hitCheck()) { return; }
        Vector3Int gridPosition = _grid.WorldToCell(transform.position);
        gridPosition += direction;
        targetPosition = _grid.GetCellCenterWorld(gridPosition);
        Debug.Log($"Moving to {gridPosition} at world position {targetPosition}");
    }
    /*
     * Checks if the enemy hits a wall or player in front of it. If it hits a wall, it reverses direction. If it hits a player, it logs a message (to be replaced with player damage).
     * @return true if a wall or player is hit, false otherwise.
     */
    private bool hitCheck()
    {
        //PlayerRay
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, new Vector3(direction.x, direction.y, 0), 1f);
        // Implement Player Checking Here

        bool playerFound = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Enemy hit the player!"); //Implement player damage here
                playerFound = true;
            }
            else if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                direction = -direction; // Reverse direction on wall hit
            }
        }

        return playerFound;
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
