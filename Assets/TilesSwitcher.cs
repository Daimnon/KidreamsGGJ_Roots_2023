using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesSwitcher : MonoBehaviour
{
    [SerializeField]
    public int tileNumber = 0;
    private Tilemap tilemap;
    [SerializeField]
    private TileBase newTile;
    private GameManager gm;

    private void Awake()
    {
        gm = GetComponent<GameManager>();
        tilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        if(newTile != null)
        OnSwitchTiles();
    }
    private void OnSwitchTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePos);
                if (tile != null)
                {
                    tilemap.SetTile(tilePos, newTile);
                }
                else
                {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
    }
}


