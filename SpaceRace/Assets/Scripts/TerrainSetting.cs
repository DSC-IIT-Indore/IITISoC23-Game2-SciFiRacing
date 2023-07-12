using UnityEngine;

[CreateAssetMenu (fileName = "TerrainSetting", menuName = "Terrain Setting")]
public class TerrainSetting : ScriptableObject
{
    [Header ("Terrain Setting")]
    public int width; // Number of vertices in the x axis
    public int length; // Number of vertices in the z axis
    public float scaleA, scaleB, scaleC; // Scale of the noise
    public float heightMultiplier; // Height multiplier of the terrain
    public float spacingIndex; // Spacing between vertices 
    public Gradient gradient; // Gradient of the terrain

    [Header("Track Generator Settings")] // Header for the inspector
    public float noiseScale; // Scale of the noise
    public float noiseHeightMultiplier; // Height multiplier of the noise
    public int trackWidth; // Width of the track in vertices
    public float edgeSmoothing; // Smoothing of the edges of the track
    public float curveResolution;
    public float curvePadding;
    
}
