[System.Serializable]
public class CharaData {
    public int id;
    public int movePower; // 移動力
    public int hp; // ヒットポイント
    public string name;

    public CharaData(int movePower, int hp) {
        this.movePower = movePower;
        this.hp = hp;
    }
}