using System.Linq;
using TMPro;
using UnityEngine;

public class Crawler : Enemy
{
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite leftSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
        changeSpriteDirection();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void movement()
    {
        if (hitCheck()) {
            changeSpriteDirection();
            //Debug.Log("Cornered and Turned Around");
            return;
        }
        Vector3Int gridPosition = _grid.WorldToCell(transform.position);
        gridPosition += direction;
        targetPosition = _grid.GetCellCenterWorld(gridPosition);

        changeSpriteDirection();

        
    }

    private void changeSpriteDirection()
    {
        if (direction == Vector3Int.right) { spriteRenderer.sprite = rightSprite; }
        else if (direction == Vector3Int.left) { spriteRenderer.sprite = leftSprite; }
    }
}
