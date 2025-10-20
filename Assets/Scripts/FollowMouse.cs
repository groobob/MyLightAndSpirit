using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private bool mouseDisabled = false;
    [SerializeField] private float angleOffset = -90f;

    void Update()
    {
        if(!mouseDisabled && playerMove != null && !playerMove.playerDroppedFlashLight() && !playerMove.didPlayerDie()) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            Vector2 direction = (mousePos - transform.position).normalized;
            
            // Rotate the object to face the mouse
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
        }
    }
}
