// just for learning / testing purposes

using UnityEngine;
using System.Collections.Generic;

public class PlayerRaycast : MonoBehaviour
{
    public float maxDistance = 100;        
    public LayerMask reflectionLayers; 
    private int reflectionLimit = 10;
    
    private float totalDegree = 40; // total degree of rays
    private float intervalDegree = 2.86f; // degree between each ray
    
    public Vector2 initialDirection = new Vector2(1, 0);
    
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>(); // original colours of SpriteRenderers that are hit
    private HashSet<SpriteRenderer> currentlyHitObjects = new HashSet<SpriteRenderer>(); // objects currently being hit
    
    
    void Update()
    {
        // reset colours of objects that are no longer being hit
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
        
        // calculate number of rays in the cone
        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1;
        float startAngle = -totalDegree / 2f;
        
        // get the base angle of the initial direction
        float baseAngle = Mathf.Atan2(initialDirection.y, initialDirection.x) * Mathf.Rad2Deg;
        
        // cast rays in cone formation
        for (int rayIndex = 0; rayIndex < numberOfRays; rayIndex++)
        {
            // calculate angle for this ray
            float currentAngle = baseAngle + startAngle + (rayIndex * intervalDegree);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            
            // create direction vector for this ray
            Vector2 rayDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            
            // cast this ray with reflections
            CastReflectedRay(rayDirection, rayIndex);
        }
    }
    
    void CastReflectedRay(Vector2 initialRayDirection, int rayIndex)
    {
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialRayDirection;
        int bounceCount = 0;
        
        // generate a consistent color for this ray based on its index
        Color rayColor = Color.HSVToRGB((rayIndex * 0.1f) % 1f, 0.8f, 1f);
        
        for (int i = 0; i < reflectionLimit; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, reflectionLayers);
            
            DrawRaySegment(currentOrigin, hit, currentDirection, maxDistance, rayColor);
            
            bool didHit = checkHit(hit, bounceCount);
            
            if(didHit)
            {
                currentOrigin = hit.point + hit.normal * 0.01f;
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                bounceCount++;
            }
            else
            {
                break; // no hit, stop reflecting this ray
            }
        }
    }

    
    void DrawRaySegment(Vector2 origin, RaycastHit2D hit, Vector2 direction, float maxDist, Color color)
    {
        if (hit.collider != null) // cut ray by object
        {
            Debug.DrawRay(origin, direction * hit.distance, color);
        }
        else // maxDist ray
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
                if (!originalColors.ContainsKey(rend)) // store original colour of object
                {
                    originalColors[rend] = rend.color;
                }
                
                currentlyHitObjects.Add(rend); // add object to currently hit objects
                rend.color = Color.green; // set object to green
            }
            return true;
        }
        else // ray didn't hit anything
        {
            Debug.Log($"Ray didn't hit anything after {bounceCount} bounces");
            return false;
        }
    }
}

