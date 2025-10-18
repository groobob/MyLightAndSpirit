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
    public static float cdDuration = 0.1f;

    [SerializeField] private Vector2Int direction = Vector2Int.right;
    [SerializeField] private GameObject flashLightObject;
    private bool droppedFlashLight = false;
    private bool playerDead = false;

    private float deathPauseTime = 2f;
    
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
        if (playerDead)
        {
            return;
        }
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dropFlashLight();
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
        Invoke(nameof(ResetMoveCD), cdDuration);

        transform.position = targetPosition;
        Vector3Int cellPosition = tilemap.WorldToCell(targetPosition + (Vector3Int)direction);
        if (wallCheck(cellPosition)) { return; }
        targetPosition = tilemap.GetCellCenterWorld(cellPosition);

        moveAllEnemies(direction);
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
                // Check WIN BLOCK
                if (interactable is NextLevel)
                {
                    Debug.Log("Player reached Next Level Block");
                    LevelManager.Instance.GetComponent<LevelManager>().GenerateNextLevel();
                }
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
            if (interactable == null && collider.gameObject.layer != LayerMask.NameToLayer("Enemies")) { Debug.Log("player should've died but interactableBlock Not Found"); return; }
            if ((collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible()) || (collider.gameObject.layer == LayerMask.NameToLayer("Enemies")))
            {
                Debug.Log($"Player Died to {collider.gameObject.name}");
                killPlayer();
            }
        }
    }

    private void blockInteraction()
    {
        if (onMoveCD) { return; }
        onMoveCD = true;
        Invoke(nameof(ResetMoveCD), cdDuration);

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
                if (interactable.plrInteractEvent(blockMovePos))
                {
                    moveAllEnemies(Vector2Int.zero);
                }
            }
        }
    }

    private void dropFlashLight()
    {
        if (!droppedFlashLight)
        {
            droppedFlashLight = true;
            flashLightObject.transform.parent = LevelManager.Instance.getCurrentLevel().transform;
            flashLightObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            droppedFlashLight = false;
            flashLightObject.transform.parent = gameObject.transform;
            flashLightObject.transform.position = gameObject.transform.position;
            flashLightObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void moveAllEnemies(Vector2Int direction)
    {
        Enemy.moveAllEnemies(LevelManager.Instance.GetComponent<LevelManager>().getCurrentLevel(), direction);
    }

    private void ResetMoveCD()
    {
        onMoveCD = false;
    }

    public bool playerDroppedFlashLight()
    {
        return droppedFlashLight;
    }

    public bool didPlayerDie()
    {
        return playerDead;
    }

    public void killPlayer()
    {
        if (playerDead)
        {
            return;
        }
        playerDead = true;
        StartCoroutine(ResetLevelAfterDelay(deathPauseTime));
    }

    IEnumerator ResetLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LevelManager.Instance.GetComponent<LevelManager>().RestartLevel();
    }
}