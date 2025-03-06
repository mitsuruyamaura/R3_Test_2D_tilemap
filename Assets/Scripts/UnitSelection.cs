using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UnitSelection : MonoBehaviour
{
    public CursorController cursorController; // カーソルの管理
    public TilemapController tilemapController; // タイルマップの移動範囲計算
    public MoveRangeVisualizer moveRangeVisualizer; // 移動範囲表示
    public CameraController cameraController;
    public GameManager gameManager;

    private CharaUnit selectedUnit;

    private void Update() {
        // キー入力でユニットを選択
        if (Input.GetKeyDown(KeyCode.Return) && gameManager.CommandState.Value == CommandStateType.CursorMove) {
            SelectUnitAtCursorPositionAsync().Forget();
        }

        // キー入力で移動先を選択
        if (selectedUnit != null && Input.GetKeyDown(KeyCode.Space)) {
            Vector3Int cursorPosition = cursorController.GetCursorPosition();
            TryMoveUnit(cursorPosition);
        }


        // UI コマンドが開いている場合、閉じる
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CommandState.Value == CommandStateType.CommandUI) {
            // 移動範囲のハイライトをクリア
            moveRangeVisualizer.ClearHighlight();

            // UI コマンドを非表示して、タイルのカーソル移動状態に戻す
            gameManager.ChangeState(CommandStateType.CursorMove);
        }
    }

    private async UniTask SelectUnitAtCursorPositionAsync() {
        Vector3Int cursorPosition = cursorController.GetCursorPosition();
        CharaUnit unitAtPosition = GetUnitAtPosition(cursorPosition);

        if (unitAtPosition != null) {
            selectedUnit = unitAtPosition; // ユニットを選択
            HighlightMoveRange(selectedUnit); // 移動範囲を表示
            cameraController.FocusOnUnit(selectedUnit.transform);
            
            await UniTask.Yield();

            // UI(コマンド、キャラステータス)表示
            gameManager.ChangeState(CommandStateType.CommandUI);

            Debug.Log($"ユニット選択 : {selectedUnit.charaData}");
        } else {
            Debug.Log("カーソル位置にユニットがいません");
        }
    }

    private CharaUnit GetUnitAtPosition(Vector3Int position) {
        // ゲーム内に配置されているすべてのユニットをチェックし、位置に対応するユニットを返す
        foreach (CharaUnit unit in FindObjectsByType<CharaUnit>(FindObjectsSortMode.None)) {
            Vector3 charaPos = unit.transform.position;
            Vector3 tilePos = tilemapController.tilemap.GetCellCenterWorld(position);
            Debug.Log($"{charaPos} : {tilePos}");
            if (charaPos == tilePos) {
                return unit;
            }
        }
        return null;
    }

    private void HighlightMoveRange(CharaUnit unit) {
        // 移動範囲を計算して表示
        Vector3Int unitPosition = tilemapController.tilemap.WorldToCell(unit.transform.position);
        List<Vector3Int> moveRange = tilemapController.CalculateMoveRange(unitPosition);
        moveRangeVisualizer.HighlightMoveRange(moveRange);
    }

    private void TryMoveUnit(Vector3Int targetPosition) {
        if (selectedUnit != null && tilemapController.CalculateMoveRange(tilemapController.tilemap.WorldToCell(selectedUnit.transform.position)).Contains(targetPosition)) {
            MoveUnitToTile(selectedUnit, targetPosition);
        } else {
            Debug.Log("移動範囲外です");
        }
    }

    private void MoveUnitToTile(CharaUnit unit, Vector3Int targetPosition) {
        unit.transform.position = tilemapController.tilemap.GetCellCenterWorld(targetPosition);
        Debug.Log("ユニットが移動しました");

        // 移動完了後、移動範囲をクリア
        moveRangeVisualizer.ClearHighlight();
    }
}