using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMove : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Values")]
    [SerializeField] float interpolationValue = 0.35f;

    Vector3 targetPosition;
    private bool onMoveCD = false;

    [SerializeField] private Vector2Int direction = Vector2Int.right;
    private void Start()
    {
        tilemap = GetComponentInParent<Tilemap>();

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
        checkDeath();
    }

    /**
     * Handles the input for the player
     */
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector2Int.up;
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector2Int.left;
            Move(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector2Int.down;
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector2Int.right;
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            blockInteraction();
        }
    }

    /**
     * Moves the player with respective to the grid
     * @param direction - the vector to move the player by
     */
    void Move(Vector2Int direction)
    {
        if (onMoveCD) { return; }
        onMoveCD = true;
        Invoke(nameof(ResetMoveCD), 0.1f);
        transform.position = targetPosition;
        Vector3Int cellPosition = tilemap.WorldToCell(targetPosition + (Vector3Int)direction);
        if (wallCheck(cellPosition)) { return; }
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);
    }
    /**
     * Checks if the player is trying to move into a wall
     * @return true if there is a wall, false otherwise
     */
    private bool wallCheck(Vector3Int cellPos)
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(tilemap.GetCellCenterWorld(cellPos), new Vector2(0.1f, 0.1f), 0);
        //GameObject wall = tilemap.GetInstantiatedObject(cellPos);
        foreach (Collider2D collider in colliderList)
        {
            //Debug.Log(collider.gameObject.layer + " | " + LayerMask.LayerToName(collider.gameObject.layer));
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("interactableBlock Not Found"); }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                
                return true;
            }
        }
        return false;
    }

    /**
     * Checks if the player is touching a deadly block
     * If so, restarts the level
     */
    private void checkDeath() {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(targetPosition, new Vector2(0.1f, 0.1f), 0);
        foreach (Collider2D collider in colliderList)
        {
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("player should've died but interactableBlock Not Found"); return; }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                Debug.Log($"Player Died to {collider.gameObject.name}");
                /*
                transform.position = LevelManager.Instance.GetComponent<LevelManager>().GetPlayerSpawnPosition();
                targetPosition = transform.position;
                */
                LevelManager.Instance.GetComponent<LevelManager>().RestartLevel();
            }
        }
    }

    private void blockInteraction()
    {
        Vector3Int pos = tilemap.WorldToCell(targetPosition + (Vector3Int) direction);
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(tilemap.GetCellCenterWorld(pos), new Vector2(0.1f, 0.1f), 0);
        foreach (Collider2D collider in colliderList)
        {
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            if (interactable == null) { Debug.Log("interactableBlock Not Found"); return; }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                //Debug.Log($"Player Interacted with {collider.gameObject.name} at {pos}");
                Vector3Int blockMovePos = new Vector3Int(direction.x, direction.y, 0);
                interactable.plrInteractEvent(blockMovePos);
            }
        }
    }

    private void ResetMoveCD()
    {
        onMoveCD = false;
    }
}