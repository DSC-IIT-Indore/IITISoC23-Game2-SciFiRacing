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
    
    public void Generate(GameObject lastChunk, GameObject currentChunk)
    {   
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;



        ProceduralTerrain lastChunkTerrain = lastChunk.GetComponent<ProceduralTerrain>();
        ProceduralTerrain currentChunkTerrain = currentChunk.GetComponent<ProceduralTerrain>();

        terrainSetting = lastChunkTerrain.terrainSetting;
        int lastChunkExit = lastChunkTerrain.trackID.ToString()[1] - '0'; 
        int currentChunkEntry = currentChunkTerrain.trackID.ToString()[0] - '0';

        vertices = new Vector3[(terrainSetting.width+1) * 2];

        UpdateMesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void GetVerticesOnEdge(int entry, int exit)
    {
        
    }

    private void UpdateMesh()
    {

    }
}
