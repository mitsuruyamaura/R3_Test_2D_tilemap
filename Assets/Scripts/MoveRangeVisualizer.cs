using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveRangeVisualizer : MonoBehaviour
{
    public Tilemap tilemap;
    public Color moveRangeColor = Color.blue;
    public Color defaultColor = Color.white;

    /// <summary>
    /// SetColor の色は
    /// </summary>
    /// <param name="moveRange"></param>
    public void HighlightMoveRange(List<Vector3Int> moveRange) {
        Debug.Log(moveRange.Count);
        foreach (var position in moveRange) {
            Vector3Int tilePosition = new (position.x, position.y, 0);
            tilemap.SetTileFlags(tilePosition, TileFlags.None);  // これを入れないと、SetColor しても色が反映されない
            tilemap.SetColor(tilePosition, moveRangeColor);
            Debug.Log(tilePosition);
        }
    }

    public void ClearHighlight() {
        //tilemap.ClearAllTiles(); // またはタイルの色をデフォルトに戻す処理

        // tilemap.cellBounds.allPositionsWithin で現在のTilemap内のすべてのグリッド座標を取得
        foreach (var position in tilemap.cellBounds.allPositionsWithin) {
            // 現在の位置にタイルが存在するか確認
            if (tilemap.HasTile(position)) {
                // タイルの色をデフォルトに戻す
                tilemap.SetColor(position, defaultColor);
            }
        }
    }
}