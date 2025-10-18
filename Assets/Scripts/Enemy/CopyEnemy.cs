using UnityEngine;
using UnityEngine.Tilemaps;

public class CopyEnemy : Enemy
{
    [SerializeField] private bool invertedHorizontal = false;
    [SerializeField] private bool invertedVertical = false;
    protected override void movement()
    {
        Vector3Int addedDirection = (Vector3Int)direction;
        if (invertedHorizontal) { addedDirection.x *= -1; }
        else if (invertedVertical) { addedDirection.y *= -1; };

            Vector3Int cellPosition = _grid.WorldToCell(targetPosition + addedDirection);
        if (hitCheck(cellPosition)) { return; }
        targetPosition = _grid.GetCellCenterWorld(cellPosition);
    }

    public void giveDirection(Vector2Int dir)
    {
        direction = (Vector3Int) dir;
    }

    void Start()
    {
        init();
    }

    protected override void Update()
    {
        base.Update();
    }

    /**
     * Function checks if the cell position is able to be moved to
     * @param cellPosition Cell Pos to move to
     * @return bool representing if allowed to move there. False = no, True = Yes
     */
    protected virtual bool hitCheck(Vector3Int cellPosition)
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(_grid.GetCellCenterWorld(cellPosition), new Vector2(0.1f, 0.1f), 0);
        foreach (Collider2D collider in colliderList)
        {
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            //if (interactable == null) { Debug.Log("interactableBlock Not Found"); }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Blocks") && interactable.isVisible())
            {
                return true;
            }
        }

        return false;
    }
}