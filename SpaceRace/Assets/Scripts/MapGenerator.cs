using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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

    public CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private List<GameObject> mapChunks = new List<GameObject>();
    public TerrainSetting terrainSetting;
    public GameObject mapChunkPrefab;

    private Vector3 spawnPosition = Vector3.zero;
    private int lastChunkID = 13;
    public int maxActiveChunks = 3;

    void Start()
    {
        terrainSetting = (TerrainSetting)Resources.Load("TerrainSetting", typeof(TerrainSetting));

        GameObject mapChunk = Instantiate(mapChunkPrefab, spawnPosition, Quaternion.identity);
        mapChunk.GetComponent<ProceduralTerrain>().Generate();
        mapChunk.transform.position = Vector3.zero;
        mapChunks.Add(mapChunk);
        spawnPosition += new Vector3(0, 0, terrainSetting.length-1); // IDK why I have to subtract 1, but it works
    }

    void Update()
    {
        // if(mapChunks.Count < maxActiveChunks)
        // {
        //     GenerateMapChunk();
        // }

        virtualCamera.enabled = true;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMapChunk();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            virtualCamera.enabled = false;
            foreach(GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                obj.transform.position -= spawnPosition * terrainSetting.spacingIndex;
            }            
            // Destroy(mapChunks[0]);
            // mapChunks.RemoveAt(0);
            spawnPosition = Vector3.zero;
        }
        
    }

    private void GenerateMapChunk()
    {
        int[] nextChunks = mapChunkDictionary[lastChunkID];
        int nextChunkID = nextChunks[Random.Range(0, nextChunks.Length)];

        GameObject mapChunk = Instantiate(mapChunkPrefab, spawnPosition, Quaternion.identity);
        mapChunk.GetComponent<ProceduralTerrain>().Generate(nextChunkID);
        Debug.Log("Chunk Generated with ID: " + nextChunkID);
        mapChunk.transform.position = Vector3.zero;
        lastChunkID = nextChunkID;
        mapChunks.Add(mapChunk);
        spawnPosition += new Vector3(0, 0, terrainSetting.length-1); // IDK why I have to subtract 1, but it works
    }
}
