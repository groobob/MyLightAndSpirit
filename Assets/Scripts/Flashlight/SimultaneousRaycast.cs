using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SimultaneousRaycast : MonoBehaviour{

    public float maxDistance = 100;
    public LayerMask reflectionLayers; // layers that can reflect rays
    
    // TODO: what value to make this?
    private int reflectionLimit = 100; // number of times to check if ray is reflected 
    
    private float totalDegree = 15; // degree of flashlight's cone
    private float intervalDegree = 2f; // degree between each ray in the cone

    
    void Update(){
        Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized;
        CastRaysInCone(direction);
    }

    bool CheckHit(RaycastHit2D hit, int reflectionCount){
        if(hit.collider != null){ // if ray hits an object
            Debug.Log($"Reflection #{reflectionCount} - Hit {hit.collider.name} at distance: {hit.distance:F2} units");
            return true;
        } 
        else{ // if ray didn't hit anything
            Debug.Log($"Ray didn't hit anything after {reflectionCount} reflections");
            return false;
        }
    }

    void DrawRay(Vector2 origin, Vector2 direction, RaycastHit2D hit, float maxDistance){
        if(hit.collider != null){ // if ray hits an object, draw ray from origin to hit point
            Debug.DrawRay(origin, direction * hit.distance, Color.green);
        }
        else{ // if ray didn't hit anything, draw ray to max distance
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
        }
    }

    void ReflectRay(Vector2 initialRayDirection, int rayIndex){
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialRayDirection;
        int reflectionCount = 0;

        for(int i = 0; i < reflectionLimit; i++){
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, reflectionLayers);
            DrawRay(currentOrigin, currentDirection, hit, maxDistance);
            bool didHit = CheckHit(hit, reflectionCount);

            if(didHit){ // if ray hits a mirror, reflect ray
                currentOrigin = hit.point + hit.normal * 0.01f;
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                reflectionCount++;
            }
            else{ // no hit, stop reflecting this ray
                break; 
            }
        }
    }

    void CastRaysInCone(Vector2 initialRayDirection){
        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1; // number of rays in the cone
        float startAngle = -totalDegree / 2f; // starting angle of the first ray
        float baseAngle = Mathf.Atan2(initialRayDirection.y, initialRayDirection.x) * Mathf.Rad2Deg; // base angle of the initial ray

        for(int rayIndex = 0; rayIndex < numberOfRays; rayIndex++){
            float currentAngle = baseAngle + startAngle + (rayIndex * intervalDegree);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;

            // direction vector for this ray
            Vector2 rayDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        
            ReflectRay(rayDirection, rayIndex);
        }
    }
}

