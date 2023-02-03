using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OsbtaclesInstantiator : MonoBehaviour
{
    [SerializeField]
    private LevelScriptable levelData;
    [SerializeField]
    private Tilemap tilemap;
    private void GenerateObstacles()
    {
        foreach (var obstacleData in levelData.obstaclesToGenerate)
        {
            tilemap.SetTile(obstacleData._obstaclePos, obstacleData._tile);
        }
    }
    private void Awake()
    {
        GenerateObstacles();
    }
}
