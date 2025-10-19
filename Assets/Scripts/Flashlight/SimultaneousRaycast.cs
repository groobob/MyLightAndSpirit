using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SimultaneousRaycast : MonoBehaviour{

    public float maxDistance = 10f;
    public float spotlightRayDistance = 0.4f;
    [SerializeField] public LayerMask hittableLayers; // layers that can reflect rays
    
    // TODO: what value to make this?
    protected int reflectionLimit = 100; // number of times to check if ray is reflected 
    
    [SerializeField] protected float totalDegree = 15; // degree of flashlight's cone
    protected float intervalDegree = 2f; // degree between each ray in the cone
    [SerializeField] bool showMissedRays = false;
    [SerializeField] PlayerMove playerMove;
    private Vector3 direction;

    [SerializeField] private GameObject lightVisualObject;
    private List<Vector3> meshVertices = new();
    private List<int> meshIndices = new();
        
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

    protected void DrawRay(Vector2 origin, Vector2 direction, RaycastHit2D hit, float maxDistance){
        if(hit){ // if ray hits an object, draw ray from origin to hit point
            Debug.DrawRay(origin, direction * hit.distance, Color.green);
        }
        else{ // if ray didn't hit anything, draw ray to max distance
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
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

    private List<Ray> CastRays(List<Ray> rays, float maxDistance)
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
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, maxDistance, hittableLayers);
            if (hits.Length == 0 && showMissedRays)
            {
                DrawRay(ray.origin, ray.direction, Physics2D.Raycast(ray.origin, ray.direction, maxDistance, hittableLayers), maxDistance);
            }
            // hit that ended up hitting the final visible block
            RaycastHit2D winningHit = new();
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                DrawRay(ray.origin, ray.direction, hit, maxDistance);
                if (!hit)
                {
                    continue;
                }
                InteractableBlock.checkRayCollision(hit);
                checkSurroundingHitPosition(hit.collider.gameObject.transform.position);
                // if it hits a visible InteractableBlock, this is the final block this ray hits and this hit wins
                if (hit.collider.gameObject.GetComponent<InteractableBlock>() && hit.collider.gameObject.GetComponent<InteractableBlock>().isVisible())
                {
                    // if hitting a different object from last, introduce null reflect ray to separate mesh here
                    if (hit.collider != lastCollider)
                    {
                        reflectRays.Add(null);
                    }
                    // if ray hits a mirror, reflect
                    if (hit.collider.gameObject.CompareTag("Mirror"))
                    {
                        reflectRays.Add(new Ray(hit.point + hit.normal * 0.01f, Vector2.Reflect(ray.direction, hit.normal)));
                    }
                    winningHit = hit;
                    break;
                }
            }
            Vector2 hitPoint = winningHit ? winningHit.point : ray.origin + ray.direction * maxDistance;
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
            lastCollider = winningHit.collider;
        }
        return reflectRays;
    }

    protected void CastRaysInCone(Vector2 initialRayDirection)
    {
        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1; // number of rays in the cone
        float startAngle = -totalDegree / 2f; // starting angle of the first ray
        float baseAngle = Mathf.Atan2(initialRayDirection.y, initialRayDirection.x) * Mathf.Rad2Deg; // base angle of the initial ray
        List<Ray> nextRays = new();
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
            nextRays = CastRays(nextRays, maxDistance);
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshIndices, 0);
        mesh.SetUVs(0, meshVertices);
        lightVisualObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    void CircularRayCasts()
    {
        int numberOfRays = 36; // number of rays in the circle
        float angleIncrement = 360f / numberOfRays; // angle between each ray
        List<Ray> nextRays = new();
        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = i * angleIncrement;
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            nextRays.Add(new Ray(transform.position, new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians))));
        }
        for (int i = 0; i < reflectionLimit; i++)
        {
            nextRays = CastRays(nextRays, spotlightRayDistance);
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

