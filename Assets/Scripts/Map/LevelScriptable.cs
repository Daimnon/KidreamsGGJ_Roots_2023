using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="TilemapData/LevelData")]
    public class LevelScriptable : ScriptableObject
    {
    public List<tileData> tilesToGenerate;
    }

[System.Serializable]
public struct tileData
{
    public Vector3Int _tilePos;
    public Tile _tile;
    public tileData(Vector3Int _tilePos, Tile tile)
    {
        this._tilePos = _tilePos;
        _tile = tile;
    }
    }
