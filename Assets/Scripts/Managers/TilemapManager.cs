using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TilemapManager : MonoBehaviour {
    public Tilemap tilemap; // タイル生成対象先の Tilemap をアサイン
    public TileSet tileSet; // スクリプタブルオブジェクトをアサイン

    private Dictionary<Vector3Int, TileInfo> tiles;

    public void GenerateTilemap(Dictionary<Vector3Int, TileInfo> tileData) {
        tiles = tileData;

        foreach (var tileEntry in tiles) {
            Vector3Int position = tileEntry.Key;
            TileInfo tileInfo = tileEntry.Value;

            // IDからTileBaseを取得
            TileBase tileBase = tileSet.GetTileBaseByID(tileInfo.TileTypeID);

            // TileBaseが見つかった場合のみタイルをセット
            if (tileBase != null) {
                tilemap.SetTile(position, tileBase);
            }
        }
    }
}