using TMPro;
using UnityEngine;

public class Crawler : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void movement()
    {
        if (hitCheck()) { return; }
        Vector3Int gridPosition = _grid.WorldToCell(transform.position);
        gridPosition += direction;
        targetPosition = _grid.GetCellCenterWorld(gridPosition);
        //Debug.Log($"Moving to {gridPosition} at world position {targetPosition}");
    }
}
