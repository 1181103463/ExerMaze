using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Collections;
using UnityEngine;

public class SaveMazeData
{
    public int width;
    public int depth;
    public float cellSize;

    public float extraConnectionChance;
    public bool generateCenterRoom;

    public int seed;
    public Vector3 playerPosition;
}