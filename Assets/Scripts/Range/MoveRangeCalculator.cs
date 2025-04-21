using System.Collections.Generic;
using UnityEngine;

public class MoveRangeCalculator
{
    private Dictionary<Vector2Int, TileInfo> tiles;

    public MoveRangeCalculator(Dictionary<Vector2Int, TileInfo> tiles) {
        this.tiles = tiles;
    }

    public HashSet<Vector2Int> CalculateMoveRange(Vector2Int start, int movePoints) {
        var moveRange = new HashSet<Vector2Int>();
        var queue = new Queue<(Vector2Int position, int remainingPoints)>();

        queue.Enqueue((start, movePoints));

        while (queue.Count > 0) {
            var (current, remainingPoints) = queue.Dequeue();

            if (moveRange.Contains(current) || remainingPoints < 0) continue;

            moveRange.Add(current);

            foreach (var direction in GetDirections()) {
                var neighbor = current + direction;
                if (tiles.TryGetValue(neighbor, out var tile)) {
                    queue.Enqueue((neighbor, remainingPoints - tile.MoveCost));
                }
            }
        }

        return moveRange;
    }

    private List<Vector2Int> GetDirections() {
        return new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
    }
}