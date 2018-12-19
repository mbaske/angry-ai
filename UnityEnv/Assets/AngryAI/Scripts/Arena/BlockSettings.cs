using UnityEngine;

[System.Serializable]
public class BlockSettings
{
    [Range(0, 100)]
    public float NumBlocks = 10f;
    [Space]
    [Range(0f, 64f)]
    public float MinDropRadius = 5f;
    [Range(0f, 64f)]
    public float MaxDropRadius = 50f;
    [Range(0f, 100f)]
    public float DropHeight = 20f;
    [Space]
    [Range(0.1f, 25f)]
    public float MinSizeX = 0.1f;
    [Range(0.1f, 25f)]
    public float MaxSizeX = 3f;
    [Range(0.1f, 25f)]
    public float MinSizeY = 0.5f;
    [Range(0.1f, 25f)]
    public float MaxSizeY = 6f;
    [Range(0.1f, 25f)]
    public float MinSizeZ = 1f;
    [Range(0.1f, 25f)]
    public float MaxSizeZ = 15f;
    [HideInInspector]
    public float MaxSize;
}