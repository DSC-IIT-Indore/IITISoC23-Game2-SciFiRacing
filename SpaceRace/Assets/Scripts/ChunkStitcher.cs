using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkStitcher : MonoBehaviour
{
    TerrainSetting terrainSetting;
    private Mesh mesh; // Mesh of the terrain
    private Vector3[] vertices; // Vertices of the terrain
    private int[] triangles; // Triangles of the terrain
    int width=0, length=2;

    public void Generate(GameObject lastChunk, GameObject currentChunk)
    {   
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


        ProceduralTerrain lastChunkTerrain = lastChunk.GetComponent<ProceduralTerrain>();
        ProceduralTerrain currentChunkTerrain = currentChunk.GetComponent<ProceduralTerrain>();

        terrainSetting = lastChunkTerrain.terrainSetting;
        int lastChunkExit = lastChunkTerrain.trackID.ToString()[1] - '0'; 
        int currentChunkEntry = currentChunkTerrain.trackID.ToString()[0] - '0';

        Debug.Log("Last chunk exit: " + lastChunkExit);
        Debug.Log("Current chunk entry: " + currentChunkEntry);

        width = terrainSetting.width;
        length = 2;
        vertices = new Vector3[(width + 1) * (length + 1)];
        triangles = new int[width * length * 6];


        // Set vertices
        int j=0, i=0;

        // Set vertices for last chunk
        while(j < terrainSetting.width+1)
        {
            if(lastChunkExit == 3){
                i = terrainSetting.length;
                vertices[j] = lastChunkTerrain.vertices[lastChunkTerrain.CoordToVert(j, i)];
            }
            else{
                if(lastChunkExit == 2) i = 0;
                else i = terrainSetting.width;
                vertices[j] = lastChunkTerrain.vertices[lastChunkTerrain.CoordToVert(i, j)];
            }
            j++;
        }

        while(j < (terrainSetting.width+1)*2)
        {
            if(currentChunkEntry == 1){
                i = 0;
                int _j = j - (terrainSetting.width+1);
                vertices[j] = currentChunkTerrain.vertices[currentChunkTerrain.CoordToVert(j, i)];
            }
            else{
                if(currentChunkEntry == 2) i = 0;
                else i = terrainSetting.width;
                int _j = j - (terrainSetting.width+1);
                vertices[j] = currentChunkTerrain.vertices[currentChunkTerrain.CoordToVert(i, j)];
            }
            j++;
        }   

        GenerateTriangles();
        UpdateMesh(lastChunkTerrain);
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void GenerateTriangles()
    {
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

    private void UpdateMesh(ProceduralTerrain chunk)
    {
        Gradient gradient = chunk.gradient;
        float minTerrainHeight = chunk.minTerrainHeight;
        float maxTerrainHeight = chunk.maxTerrainHeight;

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
}
