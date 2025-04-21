using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(fileName = "TileSet", menuName = "Create TileSet")]
public class TileSet : ScriptableObject {
    public List<TileData> tileDataList; // TileDataのリスト

    // IDに基づいてTileBaseを取得するメソッド
    public TileBase GetTileBaseByID(int id) {
        // TileData 内の TileBase を返すので、? 安全演算子を利用して TileData が null でも tileBase を null で返せるようにする
        return tileDataList.FirstOrDefault(tileData => tileData.tileID == id)?.tileBase;

        //foreach (var tileData in tiles) {
        //    if (tileData.tileID == id) {
        //        return tileData.tileBase;
        //    }
        //}
        //return null; // 該当するタイルが見つからない場合
    }
}