using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    public CharaUnit currentUnit;
    public TilemapController tilemapController;
    public MoveRangeVisualizer moveRangeVisualizer;

    private List<Vector3Int> currentMoveRange;
    private IDisposable disposable;

    public void StartUnitMove(CharaUnit unit, Vector3Int startPosition) {
        //currentUnit = unit;

        //// 移動範囲を計算
        //currentMoveRange = tilemapController.CalculateMoveRange(startPosition);

        //// 移動範囲を表示
        //moveRangeVisualizer.HighlightMoveRange(currentMoveRange);

        // 購読



    }

    public void OnTileClicked(Vector3Int clickedPosition) {
        if (currentMoveRange.Contains(clickedPosition)) {
            MoveUnitToTile(clickedPosition);
        } else {
            Debug.Log("移動範囲外です");
        }
    }

    private void MoveUnitToTile(Vector3Int targetPosition) {
        if (currentUnit != null) {
            currentUnit.transform.position = tilemapController.tilemap.GetCellCenterWorld(targetPosition);
            Debug.Log("ユニットが移動しました");

            // 移動範囲の表示をクリア
            moveRangeVisualizer.ClearHighlight();

            // 移動完了
            currentUnit.EndMove();
        }
    }
}