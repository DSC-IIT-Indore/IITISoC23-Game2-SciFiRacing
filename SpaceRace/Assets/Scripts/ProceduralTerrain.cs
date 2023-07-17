using UnityEngine;
using System.Collections;


// This script generates a procedurally generated terrain using Perlin Noise
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    public TerrainSetting terrainSetting;
    public enum TrackID{
        SouthToNorth = 13,
        SouthToEast = 14,
        SouthToWest = 12,
        EastToNorth = 43,
        EastToWest = 42,
        WestToNorth = 23,
        WestToEast = 24,

    };

    // The following variables are used to generate the terrain
    [Header("Terrain Settings")] // Header for the inspector
    public int width = 100; // Number of vertices in the x axis
    public int length = 100; // Number of vertices in the z axis
    public float scaleA, scaleB, scaleC; // Scale of the noise
    public float heightMultiplier = 10f; // Height multiplier of the terrain
    public float spacingIndex = 1; // Spacing between vertices 
    public Gradient gradient; // Gradient of the terrain

    public bool UpdateInRealTime = false; // Update the terrain in real time
    
    [Header("Track Generator Settings")] // Header for the inspector
    public float noiseScale = 10f; // Scale of the noise
    public float noiseHeightMultiplier = 10f; // Height multiplier of the noise
    public int trackWidth = 4; // Width of the track in vertices
    public float edgeSmoothing = 2f; // Smoothing of the edges of the track
    public TrackID _trackID;
    public int trackID = (int)TrackID.SouthToNorth; // ID of the track
    public float curveResolution = 0.005f;
    public float curvePadding = 0f;

    [HideInInspector]
    public Vector3 deltaSpawnPosition = Vector3.zero; // Change in spawn position due to this terrain

    private Mesh mesh; // Mesh of the terrain
    [HideInInspector]
    public Vector3[] vertices; // Vertices of the terrain
    private int[] triangles; // Triangles of the terrain
    [HideInInspector]
    public float minTerrainHeight, maxTerrainHeight; // Min and max height of the terrain

    void Awake()
    {
        terrainSetting = (TerrainSetting)Resources.Load("TerrainSettingSmall", typeof(TerrainSetting));

        minTerrainHeight = float.MinValue;
        maxTerrainHeight = float.MaxValue;

        //Set the terrain settings from the TerrainSetting scriptable object
        width = terrainSetting.width;
        length = terrainSetting.length;
        scaleA = terrainSetting.scaleA;
        scaleB = terrainSetting.scaleB;
        scaleC = terrainSetting.scaleC;
        heightMultiplier = terrainSetting.heightMultiplier;
        spacingIndex = terrainSetting.spacingIndex;
        gradient = terrainSetting.gradient;

        noiseScale = terrainSetting.noiseScale;
        noiseHeightMultiplier = terrainSetting.noiseHeightMultiplier;
        trackWidth = terrainSetting.trackWidth;
        edgeSmoothing = terrainSetting.edgeSmoothing;
        curveResolution = terrainSetting.curveResolution;
        curvePadding = terrainSetting.curvePadding;

        trackID = (int)_trackID;
        Generate(trackID);
    }

    public void Generate(int _trackID_ = 13)
    {   
        trackID = _trackID_;

        // Create the mesh and update it
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateTerrain();
        GenerateTrack(trackID);
        UpdateMinMaxHeight();
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void Update()
    {
        // If the user wants to update the terrain in real time, then update the terrain at each frame
        if(UpdateInRealTime){
            trackID = (int)_trackID;
            CreateTerrain();
            GenerateTrack(trackID);
            UpdateMinMaxHeight();
            UpdateMesh();
        }
        
    }


    // Terrain Generation Functions

    private void CreateTerrain()
    {
        vertices = new Vector3[(width + 1) * (length + 1)];
        triangles = new int[width * length * 6];
        maxTerrainHeight = 0;
        minTerrainHeight = 0;

        int vertIndex = 0;
        for (int z = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                // Generate the height of the terrain using Perlin Noise
                float _x = x + transform.position.x - width/2; // Offset the vertices so that the terrain is centered
                float _z = z + transform.position.z - length/2; // Offset the vertices so that the terrain is centered
                
                // Generate the height of the terrain using Perlin Noise
                double simplexValue = Mathf.PerlinNoise(_x * scaleA, _z * scaleA);
                double perlinValue = Mathf.PerlinNoise(_x * scaleB * 3, _z * scaleB * 3);
                double ridgedValue = Mathf.PerlinNoise(_x * scaleC * 2, _z * scaleC * 2);

                float y = (float)((simplexValue * 0.5 + perlinValue * 0.3 + ridgedValue * 0.2) * heightMultiplier);
                //y = Mathf.Pow(y, heightExponent);

                // Set the vertices
                vertices[vertIndex] = new Vector3(_x * spacingIndex, y, _z * spacingIndex);

                // Get the min and max height of the terrain
                if(y > maxTerrainHeight) maxTerrainHeight = y;
                if(y < minTerrainHeight) minTerrainHeight = y;

                vertIndex++;
            }
        }

        // Generate the triangles
        int triIndex = 0;
        int vertCount = width + 1;
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int topLeft = z * vertCount + x;
                int topRight = topLeft + 1;
                int bottomLeft = (z + 1) * vertCount + x;
                int bottomRight = bottomLeft + 1;

                triangles[triIndex] = topLeft;
                triangles[triIndex + 1] = bottomLeft;
                triangles[triIndex + 2] = topRight;
                triangles[triIndex + 3] = topRight;
                triangles[triIndex + 4] = bottomLeft;
                triangles[triIndex + 5] = bottomRight;

                triIndex += 6;
            }
        }
    }


    private void UpdateMinMaxHeight()
    {
        for(int i=0; i < vertices.Length; i++){
            minTerrainHeight = Mathf.Min(vertices[i].y, minTerrainHeight);
            maxTerrainHeight = Mathf.Max(vertices[i].y, maxTerrainHeight);
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Color[] colors = new Color[vertices.Length];
        // Set the color of the vertices based on the height of the terrain
        for (int i = 0; i < vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
            colors[i] = gradient.Evaluate(normalizedHeight);
        }
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }


    // Track generation functions

    private void GenerateTrack(int ID)
    {

        // Generate the track based on the ID
        switch(ID)
        {
            case (int)TrackID.SouthToNorth:
                GenerateSouthToNorth();
                deltaSpawnPosition = new Vector3(0, 0, length); // Change in spawn position due to this terrain
                break;
            
            case (int)TrackID.WestToEast:
                GenerateWestToEast();
                deltaSpawnPosition = new Vector3(width, 0, 0);
                break;

            case (int)TrackID.EastToWest:
                GenerateEastToWest();
                deltaSpawnPosition = new Vector3(-width, 0, 0);
                break;

            case (int)TrackID.SouthToEast:
                GenerateSouthToEast();
                deltaSpawnPosition = new Vector3(width, 0, 0);
                break;

            case (int)TrackID.SouthToWest:
                GenerateSouthToWest();
                deltaSpawnPosition = new Vector3(-width, 0, 0);
                break;

            case (int)TrackID.EastToNorth:
                GenerateEastToNorth();
                deltaSpawnPosition = new Vector3(0, 0, length);
                break;

            case (int)TrackID.WestToNorth:
                GenerateWestToNorth();
                deltaSpawnPosition = new Vector3(0, 0, length);
                break;

            default:
                GenerateSouthToNorth();
                deltaSpawnPosition = new Vector3(0, 0, length);
                break;
        }

    }

    // Utility functions
    public int CoordToVert(Vector2 coord)
    {
        int v = (int)coord.y * (width + 1) + (int)coord.x;
        v = Mathf.Clamp(v, 0, vertices.Length - 1);
        return v;
    }

    public int CoordToVert(int x, int z)
    {
        int v = z * (width + 1) + x;
        v = Mathf.Clamp(v, 0, vertices.Length - 1);
        return v;
    }

    public int OffsetX(int _vertIndex, int _offset)
    {
        return _vertIndex + _offset;
    }

    public int OffsetZ(int _vertIndex, int _offset)
    {
        return _vertIndex + _offset * (width + 1);
    }

    public Vector2 QuadraticCurve(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }


    // Track generator functions
    private void GenerateSouthToNorth()
    {
        // Generate the track from south to north

        for (int z = 0; z <= length; z++)
        {
            float _z = z + (int)transform.position.z; 

            float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            int x = width/2 + (int) (directionOffset);

            int vertIndex = z * (width + 1) + x;

            // Make the track
            vertices[vertIndex].y -= maxTerrainHeight;
            for(int i=1; i<=trackWidth/2; i++){
                vertices[OffsetX(vertIndex, +i)].y -= maxTerrainHeight;
                vertices[OffsetX(vertIndex, -i)].y -= maxTerrainHeight; 
            }

            // Smooth the edges of the track
            for(int i=trackWidth/2; i<=trackWidth; i++){
                vertices[OffsetX(vertIndex, +i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,+trackWidth/2)].y, vertices[OffsetX(vertIndex,+trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                    
                vertices[OffsetX(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,-trackWidth/2)].y, vertices[OffsetX(vertIndex,-trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
            }
            
        }

    }

    private void GenerateWestToEast()
    {
        // Generate the track from west to east

        for (int x = 0; x <= width; x++)
        {
            float _x = x + (int)transform.position.x; 

            float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

            int z = length/2 + (int) (directionOffset);

            int vertIndex = z * (width + 1) + x;

            // Make the track
            vertices[vertIndex].y -= maxTerrainHeight;
            for(int i=1; i<=trackWidth/2; i++){
                vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
            }

            // Smooth the edges of the track
            for(int i=trackWidth/2; i<=trackWidth; i++){
                vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                    
                vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
            }
            
        }

    }

    private void GenerateEastToWest()
    {
        // Generate the track from east to west

        for (int x = width; x >= 0; x--)
        {
            float _x = x + (int)transform.position.x; 

            float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

            int z = length/2 + (int) (directionOffset);

            int vertIndex = z * (width + 1) + x;

            // Make the track
            vertices[vertIndex].y -= maxTerrainHeight;
            for(int i=1; i<=trackWidth/2; i++){
                vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
            }

            // Smooth the edges of the track
            for(int i=trackWidth/2; i<=trackWidth; i++){
                vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                    
                vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
            }
            
        }

    }


    private void LowerPointsInTrackArea(Vector2 point)
    {
        int minX = Mathf.FloorToInt(point.x - trackWidth / 2);
        int maxX = Mathf.CeilToInt(point.x + trackWidth / 2);
        int minZ = Mathf.FloorToInt(point.y - trackWidth / 2);
        int maxZ = Mathf.CeilToInt(point.y + trackWidth / 2);

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int currentVertIndex = CoordToVert(new Vector2(x, z));
                vertices[currentVertIndex].y -= vertices[currentVertIndex].y > 0 ? maxTerrainHeight : 0;
            }
        }
    }

    
    private void GenerateSouthToEast()
    {
        // Generate the track from south to east

        // First generate normal track
        {
            for (int z = 0; z <= (int)length*curvePadding; z++)
            {
                float _z = z + (int)transform.position.z; 

                float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
                int x = width/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetX(vertIndex, +i)].y -= maxTerrainHeight;
                    vertices[OffsetX(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetX(vertIndex, +i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,+trackWidth/2)].y, vertices[OffsetX(vertIndex,+trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetX(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,-trackWidth/2)].y, vertices[OffsetX(vertIndex,-trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
            }

            for (int x = width - (int)(width*curvePadding); x <= width; x++)
            {
                float _x = x + (int)transform.position.x; 

                float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

                int z = length/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                    vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
                
            }
        }

        // Then generate the curve
        {
            // Find the entry point
            int x = 0, z = (int) (length*curvePadding);
            float _z = z + (int)transform.position.z;
            float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            x = width/2 + (int) (directionOffset);

            Vector2 entry = new Vector2(x, z);

            // Find the exit point
            x = width - (int)(width*curvePadding);
            float _x = x + (int)transform.position.x; 
            directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            z = length/2 + (int) (directionOffset);

            Vector2 exit = new Vector2(x, z);

            // Find the mid point
            Vector2 midPoint = new Vector2(width/2, length/2);

            float t = 0;
            Vector2 prevPoint = entry;
            while(t <= 1){
                // Find the point on the curve
                Vector2 point = QuadraticCurve(entry, midPoint, exit, t);
                Vector2 dir = (point - prevPoint);
                float delta = dir.magnitude;

                int vertIndex = CoordToVert(point);
                vertices[vertIndex].y -= vertices[vertIndex].y > 0 ? maxTerrainHeight : 0;            

                LowerPointsInTrackArea(point);

                prevPoint = point;
                t += curveResolution;
            }
        }

    }

    private void GenerateSouthToWest()
    {
        // Generate the track from south to west

        // First generate normal track
        {
            for (int z = 0; z <= (int)length*curvePadding; z++)
            {
                float _z = z + (int)transform.position.z; 

                float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
                int x = width/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetX(vertIndex, +i)].y -= maxTerrainHeight;
                    vertices[OffsetX(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetX(vertIndex, +i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,+trackWidth/2)].y, vertices[OffsetX(vertIndex,+trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetX(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,-trackWidth/2)].y, vertices[OffsetX(vertIndex,-trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
            }

            for (int x=0; x <= (int)(width*curvePadding); x++)
            {
                float _x = x + (int)transform.position.x; 

                float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

                int z = length/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                    vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
                
            }
        }

        // Then generate the curve
        {
            // Find the entry point
            int x = 0, z = (int) (length*curvePadding);
            float _z = z + (int)transform.position.z;
            float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            x = width/2 + (int) (directionOffset);

            Vector2 entry = new Vector2(x, z);

            // Find the exit point
            x = (int)(width*curvePadding);
            float _x = x + (int)transform.position.x; 
            directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            z = length/2 + (int) (directionOffset);

            Vector2 exit = new Vector2(x, z);

            // Find the mid point
            Vector2 midPoint = new Vector2(width/2, length/2);

            float t = 0;
            Vector2 prevPoint = entry;
            while(t <= 1){
                // Find the point on the curve
                Vector2 point = QuadraticCurve(entry, midPoint, exit, t);
                Vector2 dir = (point - prevPoint);
                float delta = dir.magnitude;

                int vertIndex = CoordToVert(point);
                vertices[vertIndex].y -= vertices[vertIndex].y > 0 ? maxTerrainHeight : 0;            

                LowerPointsInTrackArea(point);

                prevPoint = point;
                t += curveResolution;
            }
        }

    }

    private void GenerateEastToNorth()
    {
        // Generate the track from east to north

        // First generate normal track
        {
            for (int z = length - (int)(length*curvePadding); z <= length; z++)
            {
                float _z = z + (int)transform.position.z; 

                float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
                int x = width/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetX(vertIndex, +i)].y -= maxTerrainHeight;
                    vertices[OffsetX(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetX(vertIndex, +i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,+trackWidth/2)].y, vertices[OffsetX(vertIndex,+trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetX(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,-trackWidth/2)].y, vertices[OffsetX(vertIndex,-trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
            }

            for (int x = width - (int)(width*curvePadding); x <= width; x++)
            {
                float _x = x + (int)transform.position.x; 

                float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

                int z = length/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                    vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
                
            }
        }

        // Then generate the curve
        {
            // Find the entry point
            int x = 0, z = length - (int)(length*curvePadding);
            float _z = z + (int)transform.position.z;
            float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            x = width/2 + (int) (directionOffset);

            Vector2 entry = new Vector2(x, z);

            // Find the exit point
            x = width - (int)(width*curvePadding);
            float _x = x + (int)transform.position.x; 
            directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            z = length/2 + (int) (directionOffset);

            Vector2 exit = new Vector2(x, z);

            // Find the mid point
            Vector2 midPoint = new Vector2(width/2, length/2);

            float t = 0;
            Vector2 prevPoint = entry;
            while(t <= 1){
                // Find the point on the curve
                Vector2 point = QuadraticCurve(entry, midPoint, exit, t);
                Vector2 dir = (point - prevPoint);
                float delta = dir.magnitude;

                int vertIndex = CoordToVert(point);
                vertices[vertIndex].y -= vertices[vertIndex].y > 0 ? maxTerrainHeight : 0;            

                LowerPointsInTrackArea(point);

                prevPoint = point;
                t += curveResolution;
            }
        }

    }

    private void GenerateWestToNorth()
    {
        // Generate the track from west to north

        // First generate normal track
        {
            for (int z = length - (int)(length*curvePadding); z <= length; z++)
            {
                float _z = z + (int)transform.position.z; 

                float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
                int x = width/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetX(vertIndex, +i)].y -= maxTerrainHeight;
                    vertices[OffsetX(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetX(vertIndex, +i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,+trackWidth/2)].y, vertices[OffsetX(vertIndex,+trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetX(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetX(vertIndex,-trackWidth/2)].y, vertices[OffsetX(vertIndex,-trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
            }

            for (int x = 0; x <= (int)(width*curvePadding); x++)
            {
                float _x = x + (int)transform.position.x; 

                float directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
                directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1

                int z = length/2 + (int) (directionOffset);

                int vertIndex = z * (width + 1) + x;

                // Make the track
                vertices[vertIndex].y -= maxTerrainHeight;
                for(int i=1; i<=trackWidth/2; i++){
                    vertices[OffsetZ(vertIndex, i)].y -= maxTerrainHeight;
                    vertices[OffsetZ(vertIndex, -i)].y -= maxTerrainHeight; 
                }

                // Smooth the edges of the track
                for(int i=trackWidth/2; i<=trackWidth; i++){
                    vertices[OffsetZ(vertIndex, i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, trackWidth/2)].y, vertices[OffsetZ(vertIndex, trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                        
                    vertices[OffsetZ(vertIndex, -i)].y = Mathf.Lerp(vertices[OffsetZ(vertIndex, -trackWidth/2)].y, vertices[OffsetZ(vertIndex, -trackWidth)].y, 
                                                        edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                }
                
            }
        }

        // Then generate the curve
        {
            // Find the entry point
            int x = 0, z = length - (int)(length*curvePadding);
            float _z = z + (int)transform.position.z;
            float directionOffset = Mathf.PerlinNoise1D(_z * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            x = width/2 + (int) (directionOffset);

            Vector2 entry = new Vector2(x, z);

            // Find the exit point
            x = (int)(width*curvePadding);
            float _x = x + (int)transform.position.x; 
            directionOffset = Mathf.PerlinNoise1D(_x * noiseScale);
            directionOffset = (directionOffset*2 - 1) * noiseHeightMultiplier; // Make the value between -1 and 1
            z = length/2 + (int) (directionOffset);

            Vector2 exit = new Vector2(x, z);

            // Find the mid point
            Vector2 midPoint = new Vector2(width/2, length/2);

            float t = 0;
            Vector2 prevPoint = entry;
            while(t <= 1){
                // Find the point on the curve
                Vector2 point = QuadraticCurve(entry, midPoint, exit, t);
                Vector2 dir = (point - prevPoint);
                float delta = dir.magnitude;

                int vertIndex = CoordToVert(point);
                vertices[vertIndex].y -= vertices[vertIndex].y > 0 ? maxTerrainHeight : 0;            

                LowerPointsInTrackArea(point);

                prevPoint = point;
                t += curveResolution;
            }
        }

    }

    // End of track generation functions
}
