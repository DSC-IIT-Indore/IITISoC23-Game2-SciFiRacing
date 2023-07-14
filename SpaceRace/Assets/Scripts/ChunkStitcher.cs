using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkStitcher
{
    TerrainSetting terrainSetting;

    public void Stitch(GameObject lastChunk, GameObject currentChunk)
    {   
        ProceduralTerrain lastChunkTerrain = lastChunk.GetComponent<ProceduralTerrain>();
        ProceduralTerrain currentChunkTerrain = currentChunk.GetComponent<ProceduralTerrain>();

        terrainSetting = lastChunkTerrain.terrainSetting;
        int lastChunkExit = lastChunkTerrain.trackID.ToString()[1] - '0'; 
        int currentChunkEntry = currentChunkTerrain.trackID.ToString()[0] - '0';

        // Set vertices
        int k=0;
        int i1 = 0;
        int i2 = 0;
        int vert1, vert2;

        // Set heights equal to average of the two
        while(k < terrainSetting.width+1)
        {
            if(lastChunkExit == 3){
                i1 = terrainSetting.length;
                i2 = 0; 
                vert1 = lastChunkTerrain.CoordToVert(k, i1);
                vert2 = currentChunkTerrain.CoordToVert(k, i2);
            }
            else{
                if(lastChunkExit == 2) i1 = 0;
                else i1 = terrainSetting.width;

                if(currentChunkEntry == 2) i2 = 0;
                else i2 = terrainSetting.width;

                vert1 = lastChunkTerrain.CoordToVert(i1, k);
                vert2 = currentChunkTerrain.CoordToVert(i2, k);
            }

            float avgHeight = (lastChunkTerrain.vertices[vert1].y + currentChunkTerrain.vertices[vert2].y) / 2;
            lastChunkTerrain.vertices[vert1].y = avgHeight;
            currentChunkTerrain.vertices[vert2].y = avgHeight;
            
            k++;
        } 
        lastChunkTerrain.UpdateMesh();
        currentChunkTerrain.UpdateMesh();
    }

   
}
