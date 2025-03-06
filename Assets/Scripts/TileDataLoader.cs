using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileDataJson {
    public int x;
    public int y;
    public string TileType;
    public int MoveCost;
    public int TileTypeID;
}

public class TileDataLoader : MonoBehaviour{
    public TextAsset jsonFile; // Jsonファイルをインスペクタで設定
    private Dictionary<Vector3Int, TileInfo> tiles = new ();
    public List<TileDataJson> tileDataList = new(); 

    private void Start() {
        LoadTileDataFromJson();
    }

    public void LoadTileDataFromJson() {
        tileDataList = JsonHelper.ListFromJson<TileDataJson>(jsonFile.text);
        foreach (TileDataJson data in tileDataList) {
            Vector3Int position = new (data.x, data.y, 0);
            tiles[position] = new TileInfo {
                Position = position,
                TileType = data.TileType,
                MoveCost = data.MoveCost,
                TileTypeID = data.TileTypeID
            };
        }
    }

    public Dictionary<Vector3Int, TileInfo> GetTiles() {
        return tiles;
    }
}