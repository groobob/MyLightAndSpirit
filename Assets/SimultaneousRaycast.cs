using UnityEngine;
using System.Collections.Generic;

public class SimultaneousRaycast : MonoBehaviour
{
    public float maxDistance = 100;        
    public LayerMask reflectionLayers;
    private int reflectionLimit = 10;      
    
    public Vector2 initialDirection = new Vector2(1, 0);
    
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private HashSet<SpriteRenderer> currentlyHitObjects = new HashSet<SpriteRenderer>(); 
    
    
    void Update()
    {
        HashSet<SpriteRenderer> objectsToReset = new HashSet<SpriteRenderer>(originalColors.Keys);
        objectsToReset.ExceptWith(currentlyHitObjects);
        foreach (var renderer in objectsToReset)
        {
            if (renderer != null)
            {
                renderer.color = originalColors[renderer];
            }
        }
        
        currentlyHitObjects.Clear();
        
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialDirection;
        int bounceCount = 0;
        
        for (int i = 0; i < reflectionLimit; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, reflectionLayers);
            
            Color rayColor = Random.ColorHSV();
            
            DrawRaySegment(currentOrigin, hit, currentDirection, maxDistance, rayColor);
            
            bool didHit = checkHit(hit, bounceCount);
            
            if(didHit)
            {
                currentOrigin = hit.point + hit.normal * 0.01f;
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                bounceCount++;
            }
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

    bool checkHit(RaycastHit2D hit, int bounceCount)
    {
        if (hit.collider != null)
        {
            Debug.Log($"Bounce #{bounceCount} - Hit {hit.collider.name} at distance: {hit.distance:F2} units");
            
            SpriteRenderer rend = hit.collider.GetComponent<SpriteRenderer>();
            if (rend)
            {
                if (!originalColors.ContainsKey(rend))
                {
                    originalColors[rend] = rend.color;
                }
                
                currentlyHitObjects.Add(rend);
                rend.color = Color.green;
            }
            return true;
        }
        else
        {
            Debug.Log($"Ray didn't hit anything after {bounceCount} bounces");
            return false;
        }
    }
}
