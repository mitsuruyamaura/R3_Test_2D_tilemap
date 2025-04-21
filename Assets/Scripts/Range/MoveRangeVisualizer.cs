using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveRangeVisualizer : MonoBehaviour
{
    public Tilemap tilemap;
    public Color defaultColor = Color.white;
    public Color moveRangeColor = Color.blue;
    public Color attackRangeColor = Color.red;

    /// <summary>
    /// 移動範囲をハイライト
    /// </summary>
    /// <param name="moveRange"></param>
    public void HighlightMoveRange(HashSet<Vector3Int> moveRange) {
        Debug.Log(moveRange.Count);
        foreach (var position in moveRange) {
            Vector3Int tilePosition = new (position.x, position.y, 0);
            tilemap.SetTileFlags(tilePosition, TileFlags.None);  // これを入れないと、SetColor しても色が反映されない
            tilemap.SetColor(tilePosition, moveRangeColor);
            Debug.Log(tilePosition);
        }
    }

    /// <summary>
    /// 移動範囲や攻撃範囲のハイライトを元に戻す
    /// </summary>
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

    /// <summary>
    /// 攻撃範囲をハイライト
    /// </summary>
    /// <param name="attackRange"></param>
    public void HighlightAttackRange(HashSet<Vector3Int> attackRange) {
        Debug.Log(attackRange.Count);
        foreach (var position in attackRange) {
            Vector3Int tilePosition = new(position.x, position.y, 0);
            tilemap.SetTileFlags(tilePosition, TileFlags.None);  // これを入れないと、SetColor しても色が反映されない
            tilemap.SetColor(tilePosition, attackRangeColor);
            Debug.Log(tilePosition);
        }
    }
}