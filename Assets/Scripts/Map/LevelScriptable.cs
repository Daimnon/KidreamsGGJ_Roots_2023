using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="TilemapData/levelDataSO")]
    public class LevelData : ScriptableObject
    {
    public List<obstacleData> obstaclesToGenerate;
    }

[System.Serializable]
public struct obstacleData
{
    public Vector3Int _obstaclePos;
    public Tile _tile;
    public obstacleData(Vector3Int obstaclePos, Tile tile)
    {
        _obstaclePos = obstaclePos;
        _tile = tile;
    }
    }
