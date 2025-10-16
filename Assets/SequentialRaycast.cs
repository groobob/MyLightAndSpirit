using UnityEngine;
using System.Collections.Generic;

public class SequentialRaycast : MonoBehaviour
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

    public float maxDistance = 100;        
    public LayerMask reflectionLayers;      
    
    private Vector2 currentOrigin = new Vector2(0, 0);
    private Vector2 currentDirection = new Vector2(1, 0);
    private int bounceCount = 0;
    
    // Track original colors and currently hit objects
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private SpriteRenderer currentlyHitObject = null;
    
    
    void Update()
    {
        // Reset color of previously hit object if it's no longer being hit
        if (currentlyHitObject != null)
        {
            if (originalColors.ContainsKey(currentlyHitObject))
            {
                currentlyHitObject.color = originalColors[currentlyHitObject];
            }
            currentlyHitObject = null;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, reflectionLayers);
        
        Color rayColor = Random.ColorHSV();
        
        DrawRaySegment(currentOrigin, hit, currentDirection, maxDistance, rayColor);
        
        bool didHit = checkHit(hit);
        
        if (didHit)
        {
            currentOrigin = hit.point + hit.normal * 0.01f;
            currentDirection = Vector2.Reflect(currentDirection, hit.normal);
            bounceCount++;
            
            Debug.Log($"Next origin: {currentOrigin}, Next direction: {currentDirection}");
        }
    }

    
    void DrawRaySegment(Vector2 origin, RaycastHit2D hit, Vector2 direction, float maxDist, Color color)
    {
        if (hit.collider != null)
        {
            Debug.DrawRay(origin, direction * hit.distance, color);
        }
        else
        {
            Debug.DrawRay(origin, direction * maxDist, color);
        }
    }

    bool checkHit(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            Debug.Log($"Hit {hit.collider.name} at distance: {hit.distance:F2} units");
            
            SpriteRenderer rend = hit.collider.GetComponent<SpriteRenderer>();
            if (rend)
            {
                // Store original color if we haven't seen this object before
                if (!originalColors.ContainsKey(rend))
                {
                    originalColors[rend] = rend.color;
                }
                
                // Set as currently hit object and turn green
                currentlyHitObject = rend;
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
