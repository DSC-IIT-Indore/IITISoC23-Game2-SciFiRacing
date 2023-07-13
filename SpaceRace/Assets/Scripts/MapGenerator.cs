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
    public GameObject mapChunkPrefab, chunkStitchPrefab;
    public float maxSpawnDistance = 500f;

    private Vector3 spawnPosition = Vector3.zero;
    private int lastChunkID = 13;
    public int maxActiveChunks = 3;

    void Start()
    {
        terrainSetting = (TerrainSetting)Resources.Load("TerrainSettingSmall", typeof(TerrainSetting));

        GameObject mapChunk = Instantiate(mapChunkPrefab, spawnPosition, Quaternion.identity);
        mapChunk.GetComponent<ProceduralTerrain>().Generate();
        mapChunk.transform.position = Vector3.zero;
        mapChunks.Add(mapChunk);
        spawnPosition += mapChunk.GetComponent<ProceduralTerrain>().deltaSpawnPosition; // IDK why I have to subtract 1, but it works
    }

    void Update()
    {
        virtualCamera.enabled = true;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMapChunk();
        }

        if(mapChunks.Count > maxActiveChunks)
        {
            Destroy(mapChunks[0]);
            mapChunks.RemoveAt(0);
        }

        ResetToOrigin();
        
    }


    private void ResetToOrigin()
    {
        if(spawnPosition.sqrMagnitude > maxSpawnDistance * maxSpawnDistance)
        {
            virtualCamera.enabled = false;
            foreach(GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if(obj.layer == LayerMask.NameToLayer("Static")){
                    continue;
                }
                obj.transform.position -= spawnPosition * terrainSetting.spacingIndex;
            }            
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

        GameObject chunkStitch = Instantiate(chunkStitchPrefab, spawnPosition, Quaternion.identity);
        chunkStitch.GetComponent<ChunkStitcher>().Generate(mapChunks[mapChunks.Count-1], mapChunk);
        chunkStitch.transform.SetParent(mapChunks[mapChunks.Count-1].transform);
        chunkStitch.transform.position = Vector3.zero;

        lastChunkID = nextChunkID;
        mapChunks.Add(mapChunk);
        spawnPosition += mapChunk.GetComponent<ProceduralTerrain>().deltaSpawnPosition;
        //Debug.Log("Spawn Position: " + spawnPosition); 
    }

    
}
