using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Tilemap tilemap; // Unity の Tilemap コンポーネント
    public int maxMoveRange = 3; // 移動範囲（例: 3 マス）

    public List<Vector3Int> CalculateMoveRange(Vector3Int startPosition) {
        Debug.Log(startPosition);
        List<Vector3Int> moveRange = new List<Vector3Int>();

        // BFS を使って移動範囲を計算（例: 最大 maxMoveRange マス）
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

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
}