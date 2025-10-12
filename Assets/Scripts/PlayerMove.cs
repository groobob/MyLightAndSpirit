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
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationValue);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2.right);
        }
    }

    void Move(Vector2 direction)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(targetPosition + (Vector3)direction); 
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);
    }
}
