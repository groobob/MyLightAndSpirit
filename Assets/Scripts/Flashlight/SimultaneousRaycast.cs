using UnityEngine;
using System.Collections.Generic;

public class SimultaneousRaycast : MonoBehaviour{

    public float maxDistance = 20f;
    public float spotlightRayDistance = 0.4f;
    [SerializeField] public LayerMask hittableLayers; // layers that can reflect rays
    
    // TODO: what value to make this?
    protected int reflectionLimit = 100; // number of times to check if ray is reflected 
    
    [SerializeField] protected float totalDegree = 15; // degree of flashlight's cone
    protected float intervalDegree = 2f; // degree between each ray in the cone

    private List<Vector3> meshVertices = new();
    private List<int> meshIndices = new();
    [SerializeField] private bool doEdge = true;
    
    void Update(){
        Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized;
        CastRaysInCone(direction);
        CircularRayCasts();
    }

    protected void DrawRay(Vector2 origin, Vector2 direction, RaycastHit2D hit, float maxDistance){
        if(hit){ // if ray hits an object, draw ray from origin to hit point
            Debug.DrawRay(origin, direction * hit.distance, Color.green);
        }
        else{ // if ray didn't hit anything, draw ray to max distance
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
        }
    }

    void ReflectRay(Vector2 initialRayDirection, float distance)
    {
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialRayDirection;
        int reflectionCount = 0;

        for (int i = 0; i < reflectionLimit; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, distance, hittableLayers);
            DrawRay(currentOrigin, currentDirection, hit, distance);

            //InteractableBlock.checkRayCollision(hit);
            if (hit)
            {
                // if ray hits a mirror, reflect 
                if (hit.collider.gameObject.CompareTag("Mirror"))
                {
                    currentOrigin = hit.point + hit.normal * 0.01f;
                    currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                    reflectionCount++;
                }
                else
                {
                    InteractableBlock.checkRayCollision(hit);
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

    public class Ray
    {
        public Vector2 origin;
        public Vector2 direction;

        public Ray(Vector2 _origin, Vector2 _direction)
        {
            origin = _origin;
            direction = _direction;
        }
    }

    private List<Ray> CastRays(List<Ray> rays)
    {
        List<Ray> reflectRays = new();
        Ray lastRay = null;
        Vector2 lastHitPoint = Vector2.negativeInfinity;
        Collider2D lastCollider = null;
        foreach (Ray ray in rays)
        {
            if (ray == null)
            {
                lastHitPoint = Vector2.negativeInfinity;
                // leave lastCollider unchanged
                continue;
            }
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, hittableLayers);
            DrawRay(ray.origin, ray.direction, hit, maxDistance);
            // if hitting a different object from last, introduce null reflect ray to separate mesh here
            if (hit.collider != lastCollider)
            {
                reflectRays.Add(null);
            }
            if (hit)
            {
                // if ray hits a mirror, reflect 
                if (hit.collider.gameObject.CompareTag("Mirror"))
                {
                    reflectRays.Add(new Ray(hit.point + hit.normal * 0.01f, Vector2.Reflect(ray.direction, hit.normal)));
                }
                else
                {
                    InteractableBlock.checkRayCollision(hit);
                }
            }
            Vector2 hitPoint = hit ? hit.point : ray.origin + ray.direction * maxDistance;
            // form a quad between this raycast and previous raycast
            if (!lastHitPoint.Equals(Vector2.negativeInfinity) && lastRay != null)
            {
                Vector2 pos = new(transform.position.x, transform.position.y);
                int firstIndex = meshVertices.Count;
                meshVertices.Add(ray.origin - pos);
                meshVertices.Add(hitPoint - pos);
                meshVertices.Add(lastHitPoint - pos);
                meshVertices.Add(lastRay.origin - pos);
                meshIndices.Add(firstIndex);
                meshIndices.Add(firstIndex + 1);
                meshIndices.Add(firstIndex + 2);
                meshIndices.Add(firstIndex);
                meshIndices.Add(firstIndex + 2);
                meshIndices.Add(firstIndex + 3);
            }
            lastRay = ray;
            lastHitPoint = hitPoint;
            lastCollider = hit.collider;
        }
        return reflectRays;
    }

    protected void CastRaysInCone(Vector2 initialRayDirection)
    {
        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1; // number of rays in the cone
        float startAngle = -totalDegree / 2f; // starting angle of the first ray
        float baseAngle = Mathf.Atan2(initialRayDirection.y, initialRayDirection.x) * Mathf.Rad2Deg; // base angle of the initial ray
        List<Ray> nextRays = new List<Ray>();
        for (int rayIndex = 0; rayIndex < numberOfRays; rayIndex++)
        {
            float currentAngle = baseAngle + startAngle + (rayIndex * intervalDegree);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            nextRays.Add(new Ray(transform.position, new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians))));
        }

        meshVertices.Clear();
        meshIndices.Clear();
        for (int i = 0; i < reflectionLimit; i++)
        {
            nextRays = CastRays(nextRays);
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshIndices, 0);
        mesh.SetUVs(0, meshVertices);
        transform.GetChild(1).gameObject.GetComponent<MeshFilter>().mesh = mesh;
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
            ReflectRay(rayDirection, spotlightRayDistance);
        }
    }
}

