using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SimultaneousRaycast : MonoBehaviour{

    public float maxDistance = 100;
    public float spotlightRayDistance = 0.4f;
    public LayerMask hittableLayers; // layers that can reflect rays
    
    // TODO: what value to make this?
    protected int reflectionLimit = 100; // number of times to check if ray is reflected 
    
    protected float totalDegree = 15; // degree of flashlight's cone
    protected float intervalDegree = 2f; // degree between each ray in the cone
    [SerializeField] bool showMissedRays = false;
    [SerializeField] PlayerMove playerMove;
    private Vector3 direction;

        
    private bool mouseDisabled = false;

    private void Start()
    {
        GameManager.Instance.OnPause += Raycasts_OnPause;
        GameManager.Instance.OnUnpause += Raycasts_OnUnpause;
    }

    private void Raycasts_OnUnpause(object sender, System.EventArgs e)
    {
        mouseDisabled = false;
    }

    private void Raycasts_OnPause(object sender, System.EventArgs e)
    {
        mouseDisabled = true;
    }

    void Update(){
        if(!mouseDisabled && !playerMove.playerDroppedFlashLight() && !playerMove.didPlayerDie()) {
          direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized;
        }
        CastRaysInCone(direction);
        CircularRayCasts();
    }

    protected bool CheckHit(RaycastHit2D hit, int reflectionCount){
        if(hit.collider != null){ // if ray hits an object
            //Debug.Log($"Reflection #{reflectionCount} - Hit {hit.collider.name} at distance: {hit.distance:F2} units");
            return true;
        } 
        else{ // if ray didn't hit anything
            //Debug.Log($"Ray didn't hit anything after {reflectionCount} reflections");
            return false;
        }
    }

    protected void DrawRay(Vector2 origin, Vector2 direction, RaycastHit2D hit, float maxDistance){
        if(hit.collider != null){ // if ray hits an object, draw ray from origin to hit point
            Debug.DrawRay(origin, direction * hit.distance, Color.green);
        }
        else{ // if ray didn't hit anything, draw ray to max distance
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
        }
    }

    void ReflectRay(Vector2 initialRayDirection, int rayIndex, float distance)
    {
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialRayDirection;
        int reflectionCount = 0;
        for (int i = 0; i < reflectionLimit; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentOrigin, currentDirection, distance, hittableLayers);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            if (hits.Length == 0 && showMissedRays)
            {
                DrawRay(currentOrigin, currentDirection, Physics2D.Raycast(currentOrigin, currentDirection, distance), distance);
            }

            foreach (RaycastHit2D hit in hits)
            {
                DrawRay(currentOrigin, currentDirection, hit, distance);
                bool didHit = CheckHit(hit, reflectionCount);
                //InteractableBlock.checkRayCollision(hit);
                if (didHit)
                { // if ray hits a mirror, reflect 
                    
                    if (hit.collider.gameObject.CompareTag("Mirror"))
                    {
                        currentOrigin = hit.point + hit.normal * 0.01f;
                        currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                        reflectionCount++;
                    }
                    else
                    {
                        InteractableBlock.checkRayCollision(hit);
                        checkSurroundingHitPosition(hit.collider.gameObject.transform.position);
                    }

                    if (hit.collider.gameObject.GetComponent<InteractableBlock>() && hit.collider.gameObject.GetComponent<InteractableBlock>().isVisible())
                    {
                        break; // stop reflecting this ray if it hits a visible InteractableBlock
                    }
                }
                else
                { // no hit, stop reflecting this ray
                    break;
                }
            }
        }
    }

    protected void CastRaysInCone(Vector2 initialRayDirection){
        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1; // number of rays in the cone
        float startAngle = -totalDegree / 2f; // starting angle of the first ray
        float baseAngle = Mathf.Atan2(initialRayDirection.y, initialRayDirection.x) * Mathf.Rad2Deg; // base angle of the initial ray

        for(int rayIndex = 0; rayIndex < numberOfRays; rayIndex++){
            float currentAngle = baseAngle + startAngle + (rayIndex * intervalDegree);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;

            // direction vector for this ray
            Vector2 rayDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        
            ReflectRay(rayDirection, rayIndex, maxDistance);
        }
    }

    void CircularRayCasts()
    {
        int numberOfRays = 36; // number of rays in the circle
        float angleIncrement = 360f / numberOfRays; // angle between each ray
        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = i * angleIncrement;
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            // direction vector for this ray
            Vector2 rayDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            ReflectRay(rayDirection, i, spotlightRayDistance);
        }
    }

    /**
     * For when two blocks are stack ontop of eachother, both get shined
     */
    void checkSurroundingHitPosition(Vector3 pos)
    {
        Collider2D[] colliderList = Physics2D.OverlapBoxAll(pos, new Vector2(0.8f, 0.8f), 0);
        foreach (Collider2D collider in colliderList)
        {
            //InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            InteractableBlock.checkRayCollision(collider);
        }
    }
}

