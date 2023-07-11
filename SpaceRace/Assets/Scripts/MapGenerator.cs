using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    private Dictionary<int, int[]> mapChunkDictionary = new Dictionary<int, int[]>(){
        {13, new int[] {13, 14, 12}},
        {12, new int[] {43, 42}},
        {14, new int[] {23, 24}},

        {23, new int[] {13, 14, 12}},
        {24, new int[] {24, 23}},
        
        {43, new int[] {13, 14, 12}},
        {42, new int[] {43, 42}},
    };

    private List<GameObject> mapChunks = new List<GameObject>();
    public TerrainSetting terrainSetting;
    public GameObject mapChunkPrefab;

    private Vector3 spawnPosition = Vector3.zero;
    public GameObject currentActiveChunk;
    public int maxActiveChunks = 3;

    void Start()
    {
        terrainSetting = (TerrainSetting)Resources.Load("TerrainSetting", typeof(TerrainSetting));
    }

    void Update()
    {
        if(mapChunks.Count < maxActiveChunks)
        {
            GenerateMapChunk();
            Debug.Log(mapChunkDictionary[13][0]);
        }
    }

    private void GenerateMapChunk()
    {
        GameObject mapChunk = Instantiate(mapChunkPrefab, spawnPosition, Quaternion.identity);
        mapChunk.GetComponent<ProceduralTerrain>().Generate();
        mapChunk.transform.position = Vector3.zero;
        mapChunks.Add(mapChunk);
        spawnPosition += new Vector3(0, 0, terrainSetting.length-1); // IDK why I have to subtract 1, but it works
    }
}
