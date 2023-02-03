using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Extensions;
//Create new script


public class TilesLevelWriter : MonoBehaviour
{
    [SerializeField]
    private LevelScriptable levelData;

    [SerializeField]
    protected Vector2 tilesGenerationSize;
    [SerializeField]
    private Tile[] obstaclesToSpawn;


    private void Awake()
    {
        tilesGenerationSize = new Vector2(5, 5);
        WriteTilesData();
    }
    private void WriteTilesData()
    {
        levelData.obstaclesToGenerate.Clear();
        for (int x = 0; x < tilesGenerationSize.x; x++)
        {
            for (int y = 0; y < tilesGenerationSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x*2, y*2,0);
                var obstacle = new obstacleData(tilePos , RandomTile());
                levelData.obstaclesToGenerate.Add(obstacle);
            }
        }
    }

    private Tile RandomTile()
    {
        var random = UnityEngine.Random.Range(0, obstaclesToSpawn.Length);
        return obstaclesToSpawn[random];
    }
}
