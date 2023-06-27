using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    public int width = 100;
    public int length = 100;
    public float scale = 10f;
    public float heightMultiplier = 10f;
    public float spacingIndex = 1;
    public Gradient gradient;
    public bool UpdateInRealTime = false;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float minTerrainHeight, maxTerrainHeight;
    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateTerrain();
        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void Update()
    {
        if(UpdateInRealTime){
            CreateTerrain();
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
                float _x = x + (int)transform.position.x;     // Adjusted x, z
                float _z = z + (int)transform.position.z;

                float y = Mathf.PerlinNoise(_x * scale, _z * scale) * heightMultiplier;
                float y1 = Mathf.PerlinNoise(_x * scale*3, _z * scale*3) * heightMultiplier/4;

                y += y1;
                vertices[vertIndex] = new Vector3(_x*spacingIndex, y, _z*spacingIndex);

                if(y > maxTerrainHeight) maxTerrainHeight = y;
                if(y < minTerrainHeight) minTerrainHeight = y;
                vertIndex++;
            }
        }

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

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
            colors[i] = gradient.Evaluate(normalizedHeight);
        }
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

}
