using UnityEngine;
using UnityEngine.Tilemaps;

public class CopyEnemy : Enemy
{
    protected override void movement()
    {
        Vector3Int cellPosition = _grid.WorldToCell(targetPosition + (Vector3Int)direction);
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
}