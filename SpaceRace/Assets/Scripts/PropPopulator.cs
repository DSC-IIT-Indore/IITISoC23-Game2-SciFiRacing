using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPopulator : MonoBehaviour
{

    public ProceduralTerrain terrain;
    private int width, length;
    public float noiseScale = 0.1f;
    public float noiseThreshold = 0.7f;

    public GameObject[] props;

    void Update()
    {
        terrain = GetComponent<ProceduralTerrain>();
        width = terrain.width;
        length = terrain.length;

        Populate();    
    }

    public void Populate()
    {
        for(int x=0; x <= width; x+=5){
            
            for(int y=0; y <= length; y+=5){
                int vertIndex = terrain.CoordToVert(x, y);
                float normalizedHeight = terrain.vertices[vertIndex].y / (terrain.maxTerrainHeight - terrain.minTerrainHeight);
                
                if(normalizedHeight > noiseThreshold){    
                    int propIndex = Random.Range(0, props.Length);
                    GameObject prop = Instantiate(props[propIndex], terrain.vertices[vertIndex], Quaternion.identity);
                    prop.transform.parent = transform;
                }
            }

        }
    }
}
