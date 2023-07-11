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
    public float heightExponent = 1f; // Height exponent of the terrain
    public float spacingIndex = 1; // Spacing between vertices 
    public Gradient gradient; // Gradient of the terrain

    public bool UpdateInRealTime = false; // Update the terrain in real time
    
    [Header("Track Generator Settings")] // Header for the inspector
    [Range(0, 0.5f)]
    public float noiseScale = 10f; // Scale of the noise
    public float noiseHeightMultiplier = 10f; // Height multiplier of the noise
    public int trackWidth = 4; // Width of the track in vertices
    public float edgeSmoothing = 2f; // Smoothing of the edges of the track
    public TrackID _trackID;
    private int trackID = (int)TrackID.SouthToNorth; // ID of the track

    private Mesh mesh; // Mesh of the terrain
    private Vector3[] vertices; // Vertices of the terrain
    private int[] triangles; // Triangles of the terrain
    private float minTerrainHeight, maxTerrainHeight; // Min and max height of the terrain
    

    void Awake()
    {
        Generate();
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }

    public void Generate(int _trackID_ = 13)
    {   
        trackID = _trackID_;
        terrainSetting = (TerrainSetting)Resources.Load("TerrainSetting", typeof(TerrainSetting));
        //Set the terrain settings from the TerrainSetting scriptable object
        width = terrainSetting.width;
        length = terrainSetting.length;
        scaleA = terrainSetting.scaleA;
        scaleB = terrainSetting.scaleB;
        scaleC = terrainSetting.scaleC;
        heightMultiplier = terrainSetting.heightMultiplier;
        spacingIndex = terrainSetting.spacingIndex;
        gradient = terrainSetting.gradient;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateTerrain();
        GenerateTrack(trackID);
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
            UpdateMesh();
        }
        
    }

    private void CreateTerrain()
    {
        vertices = new Vector3[(width + 1) * (length + 1)];
        triangles = new int[width * length * 6];

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

    private void GenerateTrack(int ID)
    {
        // Pick the midpoint of a random edge of the terrain, and make it the starting point of the track (the first node). 
        // Now pick a random edge of the terrain, and make it the ending point of the track (the last node).
        // Now generate a random path between the first and last nodes. That's the track.
        
    
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
                vertices[vertIndex+i].y -= maxTerrainHeight;
                vertices[vertIndex-i].y -= maxTerrainHeight; 
            }

            // Smooth the edges of the track
            for(int i=trackWidth/2; i<=trackWidth; i++){
                vertices[vertIndex+i].y = Mathf.Lerp(vertices[vertIndex+trackWidth/2].y, vertices[vertIndex+trackWidth].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
                                                    
                vertices[vertIndex-i].y = Mathf.Lerp(vertices[vertIndex-trackWidth/2].y, vertices[vertIndex-trackWidth].y, 
                                                    edgeSmoothing*((i-trackWidth/2)/(trackWidth/2f)));
            }
            
        }

    }

}
