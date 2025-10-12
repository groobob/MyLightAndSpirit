using UnityEngine;

public class PlayerRaycast2D : MonoBehaviour
{
    /*
    A raycast is a vector: origin (point in space) + direction (unit vector)
    The direction is a unit vector, so the length of the vector is 1.
    
    - maxDistance is the length of the raycast
    - layersToHit is a bitmask of layers to hit
    - `hit` is a RaycastHit2D object that contains information about the object that was hit by the raycast
        - hit.collider: the collider that was hit
        - hit.point: the point where the raycast hit the collider
        - hit.distance: the distance from the origin to the point where the raycast hit the collider
        - hit.normal: the normal of the surface where the raycast hit the collider
        - hit.transform: the transform of the GameObject that was hit
    */

    public float maxDistance = 100;         // how far the ray shoots
    public LayerMask reflectionLayers;       // which layers of objects to hit
    
    private Vector2 currentOrigin = new Vector2(0, 0);
    private Vector2 currentDirection = new Vector2(1, 0); // initial direction
    private int bounceCount = 0;
    
    
    void Update()
    {
        // cast ONE ray per frame from current position in current direction
        RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, reflectionLayers);
        
        // use random color for this bounce
        Color rayColor = Random.ColorHSV();
        
        // draw this ray segment
        DrawRaySegment(currentOrigin, hit, currentDirection, maxDistance, rayColor);
        
        // check if we hit something
        bool didHit = checkHit(hit);
        
        if (didHit)
        {
            // prepare for next bounce in the next frame
            // offset the start point slightly away from the surface to avoid hitting the same object
            currentOrigin = hit.point + hit.normal * 0.01f;
            currentDirection = Vector2.Reflect(currentDirection, hit.normal);
            bounceCount++;
            
            Debug.Log($"Bounce #{bounceCount} - Next origin: {currentOrigin}, Next direction: {currentDirection}");
        }
        // if we didn't hit anything, just keep the current state (ray will continue in same direction next frame)
    }

    
    void DrawRaySegment(Vector2 origin, RaycastHit2D hit, Vector2 direction, float maxDist, Color color)
    {
        if (hit.collider != null)
        {
            // draw ray only to the hit point
            Debug.DrawRay(origin, direction * hit.distance, color);
        }
        else
        {
            // draw full remaining distance if nothing was hit
            Debug.DrawRay(origin, direction * maxDist, color);
        }
    }

    bool checkHit(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            Debug.Log($"Hit {hit.collider.name} at distance: {hit.distance:F2} units");
            
            // turn the hit object green (you can change this to different colors per bounce if desired)
            SpriteRenderer rend = hit.collider.GetComponent<SpriteRenderer>();
            if (rend)
            {
                rend.color = Color.green;
            }
            return true;
        }
        else
        {
            Debug.Log("Initial ray didn't hit anything");
            return false;
        }
    }
}