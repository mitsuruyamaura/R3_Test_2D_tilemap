using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharaData {
    public int charaId;
    public int movePower; // 移動力
    public int hp;        // 初期ヒットポイント。ここには直接加算・減算させない
    public string name;

    public int attack;
    public int defense;
    public int speed;
    public int technic;   // 技術力
    public int luck;
    public int range;     // 攻撃(反撃)範囲
    public int evasion;   // 回避

    public List<SkillType> skillList = new();

    public List<int> skillIdList = new();

    public List<ISkillEffect> skillEffectList = new(); // スキルリスト

    public OwnerType ownerType;

    public CharaData(int movePower, int hp) {
        this.movePower = movePower;
        this.hp = hp;
    }
}