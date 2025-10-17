using UnityEngine;

public class ShineLight : SimultaneousRaycast{
    [Header("Visual Light Reflection")] // default values
    public float beamWidth = 0.2f; 
    public Color startColor = Color.white; 
    public Color endColor = Color.white;
    
    private LineRenderer[] lightBeams;
    private Material beamMaterial;
    private int maxTotalBeams = 50; // number of beams generated
    private float lightAlpha = 0.2f;

    void Start(){
        intervalDegree = 0.25f; // interval between raycasts. smaller interval for no gaps
    
        CreateBeamMaterial();
        lightBeams = new LineRenderer[maxTotalBeams];
        for(int i = 0; i < maxTotalBeams; i++){
            GameObject beamObj = new GameObject($"LightBeam_{i}");
            beamObj.transform.SetParent(transform);

            LineRenderer lr = beamObj.AddComponent<LineRenderer>();
            lr.material = beamMaterial;
            lr.startWidth = beamWidth;
            lr.endWidth = beamWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.sortingLayerName = "Default";
            lr.sortingOrder = 10;

            Color beamColor = startColor;
            beamColor.a = lightAlpha;
            lr.startColor = startColor;
            lr.endColor = endColor;

            lr.textureMode = LineTextureMode.Stretch;
            lr.enabled = false;
            lightBeams[i] = lr;
        }
    }


    void CreateBeamMaterial(){
        int texWidth = 64; // texture
        int texHeight = 40; 
        Texture2D gradientTex = new Texture2D(texWidth, texHeight);

        for(int x = 0; x < texWidth; x++){
            for(int y = 0; y < texHeight; y++){
                gradientTex.SetPixel(x, y, new Color(1f, 1f, 1f, 1f)); 
                // we could make a gradient for the beam 
            }
        }
        gradientTex.Apply();

        beamMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        if(beamMaterial.shader == null || beamMaterial.shader.name == "Hidden/InternalErrorShader"){
            beamMaterial = new Material(Shader.Find("Particles/Additive"));
        }
        if(beamMaterial.shader == null || beamMaterial.shader.name == "Hidden/InternalErrorShader"){
            beamMaterial = new Material(Shader.Find("Unlit/Transparent"));
        }
        beamMaterial.mainTexture = gradientTex;
        beamMaterial.SetColor("_BaseColor", startColor);
        beamMaterial.SetColor("_Color", endColor);


        //beamMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        beamMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        beamMaterial.SetInt("_ZWrite", 0);
        beamMaterial.renderQueue = 3000;
    }

    void Update(){
        // colour each beam
        Color beamColor = startColor;
        beamColor.a = lightAlpha;
        foreach (LineRenderer beam in lightBeams){
            if (beam != null)
                beam.startColor = beamColor;
                beam.endColor = beamColor;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // why not use vector2?
        
        Vector3 direction = (mouseWorldPos - transform.position).normalized;
        
        // prevent issues if the mouse is exactly at object's position
        if(direction != Vector3.zero){ 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        CastBeams(direction);
    }

    void CastBeams(Vector2 initialRayDirection){
        foreach(LineRenderer beam in lightBeams){
            if(beam != null)
                beam.enabled = false;
            }

        int numberOfRays = Mathf.FloorToInt(totalDegree / intervalDegree) + 1;
        float startAngle = -totalDegree / 2f;
        float baseAngle = Mathf.Atan2(initialRayDirection.y, initialRayDirection.x) * Mathf.Rad2Deg;
        int globalBeamIndex = 0;

        // cast the beams in a cone. thx claude for this
        for(int rayIndex = 0; rayIndex < numberOfRays; rayIndex++){
            float currentAngle = baseAngle + startAngle + (rayIndex * intervalDegree);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            Vector2 rayDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            TraceRayWithReflections(rayDirection, ref globalBeamIndex);
        }
    }

    void TraceRayWithReflections(Vector2 initialDirection, ref int globalBeamIndex){
        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = initialDirection.normalized;
        int reflectionCount = 0;

        for(int reflection = 0; reflection < reflectionLimit && globalBeamIndex < maxTotalBeams; reflection++){
            RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxDistance, hittableLayers);
        
            if(lightBeams[globalBeamIndex] != null){
                lightBeams[globalBeamIndex].enabled = true;
                if(globalBeamIndex == 0){
                    lightBeams[globalBeamIndex].startWidth = beamWidth * 0.1f;
                    lightBeams[globalBeamIndex].endWidth = beamWidth;
                }
                else{
                    lightBeams[globalBeamIndex].startWidth = beamWidth;
                    lightBeams[globalBeamIndex].endWidth = beamWidth;
                }

                Vector3 startPos = new Vector3(currentOrigin.x, currentOrigin.y, 0);
                Vector3 endPos;

                if(hit.collider != null){
                    endPos = new Vector3(hit.point.x, hit.point.y, 0);
                    lightBeams[globalBeamIndex].SetPosition(0, startPos);
                    lightBeams[globalBeamIndex].SetPosition(1, endPos);

                    currentOrigin = hit.point + hit.normal * 0.01f;
                    currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                    reflectionCount++;
                }
                else{
                    endPos = startPos + new Vector3(currentDirection.x, currentDirection.y, 0) * maxDistance;
                    lightBeams[globalBeamIndex].SetPosition(0, startPos);
                    lightBeams[globalBeamIndex].SetPosition(1, endPos);

                    globalBeamIndex++;
                    break;
                }
            }

            globalBeamIndex++;
            
            if(hit.collider == null){
                break;
            }
        }
    }
}