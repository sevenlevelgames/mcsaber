using UnityEngine;

[System.Serializable]
public class BlockData
{
    [Header("Spawn Timer")]
    public float spawnTime; 

    [Header("Blok Types")]
    public BlockType type;
    public float sustainDuration;

    [Header("Lane&Color")]
    public CubeLane lane; 
    public CubeColor color;

    public Vector3 rotation; 
}


public enum BlockType
{
    Normal,
    Sustain
}

public enum CubeLane // +- 1.5 laneleri
{
    Left,
    Right 
}

public enum CubeColor
{
    Red,
    Blue
}