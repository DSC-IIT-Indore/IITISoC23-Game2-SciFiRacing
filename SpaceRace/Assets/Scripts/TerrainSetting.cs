using UnityEngine;

[CreateAssetMenu (fileName = "TerrainSetting", menuName = "Terrain Setting")]
public class TerrainSetting : ScriptableObject
{
    public int width; // Number of vertices in the x axis
    public int length; // Number of vertices in the z axis
    public float scaleA, scaleB, scaleC; // Scale of the noise
    public float heightMultiplier; // Height multiplier of the terrain
    public float spacingIndex; // Spacing between vertices 
    public Gradient gradient; // Gradient of the terrain
}
