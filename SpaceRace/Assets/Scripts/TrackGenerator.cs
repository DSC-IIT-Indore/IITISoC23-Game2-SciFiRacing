using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public ProceduralTerrain terrainGenerator;
    void Start()
    {
        terrainGenerator = GetComponent<ProceduralTerrain>();        
    }

    void Update()
    {
        
    }
}
