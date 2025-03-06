using UnityEngine;

[System.Serializable]
public class TileInfo
{
    public Vector3Int Position; // タイルの座標
    public string TileType;     // タイルの種類
    public int MoveCost;        // 移動コスト
    public int TileTypeID;
}