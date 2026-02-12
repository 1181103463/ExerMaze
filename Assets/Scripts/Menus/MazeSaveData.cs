using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MazeSaveData
{
    public int width;
    public int depth;
    public float cellSize;
    public float extraConnectionChance;
    public bool generateCenterRoom;

    public Vector3 playerPosition;

    public int randomSeed;

    public List<Vector3> itemPositions = new List<Vector3>();
}