using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tilemap tilemap;

    [Header("Values")]
    [SerializeField] float interpolationValue = 0.35f;

    Vector3 targetPosition;
    private void Start()
    {
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(cellPosition);
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);
    }

    private void FixedUpdate()
    {
        // Interpolation of player movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);
    }

    void Update()
    {
        HandleInput();
    }

    /**
     * Handles the input for the player
     */
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    /**
     * Moves the player with respective to the grid
     * @param direction - the vector to move the player by
     */
    void Move(Vector2Int direction)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(targetPosition + (Vector3Int)direction); 
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);
    }
}
