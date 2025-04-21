using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : AbstractSingleton<TilemapController>
{
    public Tilemap tilemap; // Unity の Tilemap コンポーネント
    //public int maxMoveRange = 3; // 移動範囲（例: 3 マス）

    public HashSet<Vector3Int> CalculateMoveRange(Vector3Int startPosition, int maxMoveRange) {
        Debug.Log(startPosition);
        HashSet<Vector3Int> moveRange = new();

        // BFS を使って移動範囲を計算（例: 最大 maxMoveRange マス）
        Queue<Vector3Int> queue = new();
        HashSet<Vector3Int> visited = new();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        while (queue.Count > 0) {
            Vector3Int current = queue.Dequeue();

            // 上下左右の隣接タイルを確認
            foreach (Vector3Int direction in GetAdjacentDirections()) {
                Vector3Int neighbor = current + direction;

                if (!visited.Contains(neighbor) && tilemap.HasTile(neighbor)) {
                    int moveCost = GetTileMoveCost(neighbor); // タイルの移動コストを取得
                    if (GetManhattanDistance(startPosition, neighbor) <= maxMoveRange) {
                        moveRange.Add(neighbor);
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        return moveRange;
    }

    private List<Vector3Int> GetAdjacentDirections() {
        return new List<Vector3Int>
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
    }

    private int GetTileMoveCost(Vector3Int position) {
        // タイルの移動コストを取得（デフォルト 1）
        // 必要に応じて TileBase やカスタムデータから取得
        return 1;
    }

    private int GetManhattanDistance(Vector3Int a, Vector3Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    public HashSet<Vector3Int> CalculateAttackRange(CharaUnit unit) {
        HashSet<Vector3Int> attackableTiles = new();
        Queue<Vector3Int> queue = new();
        HashSet<Vector3Int> visited = new();

        Vector3Int unitGridPos = unit.GetGridPosition(tilemap);
        queue.Enqueue(unitGridPos);
        visited.Add(unitGridPos);

        while (queue.Count > 0) {
            Vector3Int current = queue.Dequeue();
            int currentDistance = Mathf.Abs(current.x - unitGridPos.x) + Mathf.Abs(current.y - unitGridPos.y);

            if (currentDistance > unit.charaData.range) continue; // 射程オーバーならスキップ

            attackableTiles.Add(current);

            // 4方向のチェック
            Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

            foreach (var dir in directions) {
                Vector3Int next = current + dir;
                if (!visited.Contains(next) && tilemap.HasTile(next)) {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return attackableTiles;
    }

    public int GetTileDistance(Vector2Int start, Vector2Int end) {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    public Vector2Int GetCursorGridPosition() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return (Vector2Int)cellPosition;
    }


    public Vector3Int GetGridPositionFromWorld(Vector3 worldPosition) {
        return tilemap.WorldToCell(worldPosition);
    }

    // overload: Transform を直接受け取るバージョン
    public Vector3Int GetGridPositionFromTransform(Transform target) {
        return tilemap.WorldToCell(target.position);
    }

    // overload: CharaUnit を渡すバージョン（必要なら）
    public Vector3Int GetGridPositionFromUnit(CharaUnit unit) {
        return tilemap.WorldToCell(unit.transform.position);
    }


    public HashSet<Vector3Int> CalculateRange(Vector3Int startPos, int maxRange, Func<Vector3Int, bool> isTileValid = null) {
        HashSet<Vector3Int> result = new();
        Queue<Vector3Int> queue = new();
        HashSet<Vector3Int> visited = new();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0) {
            Vector3Int current = queue.Dequeue();
            int distance = GetManhattanDistance(startPos, current);

            if (distance > maxRange)
                continue;

            // 条件を満たさなければ範囲に追加（= 有効なタイル）
            if (isTileValid == null || isTileValid(current) == false) {
                result.Add(current);
            }

            foreach (Vector3Int dir in GetAdjacentDirections()) {
                Vector3Int neighbor = current + dir;
                if (!visited.Contains(neighbor) && tilemap.HasTile(neighbor)) {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return result;
    }
}