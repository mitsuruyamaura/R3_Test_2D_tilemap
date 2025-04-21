using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum UnitActionState {
    NotActed,    // 未行動
    Acted,       // 行動済
    Unactionable // 行動不可
}

public enum UnitMoveState {
    NotMoved, // 未移動
    Moved,    // 移動済
    Unmovable // 移動不可
}

public enum OwnerType {
    Player,
    Enemy
}

[System.Serializable]
public class BattlePreviewData {
    public CharaUnit attacker;
    public CharaUnit defender;
    public SkillData skillUsed;
    public int damage;
    public int hit;
    public int crit;
    public bool isCounter;
    public OwnerType ownerType;
}


public class CharaUnit : MonoBehaviour {
    public CharaData charaData;
    public ReactiveProperty<UnitActionState> ActionState = new();
    public ReactiveProperty<UnitMoveState> MoveState = new();
    public OwnerType ownerType;
    public int hp;
    public int selectSkillId;

    [SerializeField] private BattleAnimator battleAnimator;

    public bool IsAlive => hp > 0;


    public void Initialize(int movePower, int hp) {
        charaData = new CharaData(movePower, hp);
    }

    public void Initialize(CharaData charaData) {
        this.charaData = charaData;


        // TODO ターン購読
        // プレイヤーのターンになったらリフレッシュする

    }


    public void CalcHp(int amount) {
        hp += amount;
        hp = Mathf.Max(0, hp);

        if (hp <= 0) {
            gameObject.SetActive(false);

            // TODO キャラのリストから削除

        }
    }

    public void Move(Vector3Int movePos) {
        //Vector3Int targetPos = (Vector3Int)transform.position + movePos;


    }


    public void RefleshState() {
        SetActionState(UnitActionState.NotActed);
        SetMoveState(UnitMoveState.NotMoved);
    }

    public void SetActionState(UnitActionState state) {
        ActionState.Value = state;
    }

    public void SetMoveState(UnitMoveState state) {
        MoveState.Value = state;
    }

    /// <summary>
    /// 行動可能かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsActionable() {
        return ActionState.Value == UnitActionState.NotActed;
    }

    /// <summary>
    /// 移動可能かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsMovable() {
        return MoveState.Value == UnitMoveState.NotMoved;
    }

    public void EndAction() {
        ActionState.Value = UnitActionState.Acted;

        // コマンド UI 表示


        // 選択したグループのコマンドを選択不可にする


    }


    public void EndMove() {
        MoveState.Value = UnitMoveState.Moved;

        // TODO コマンド UI 表示
        // CommandStateType を CommandUI に切り替えることで、UI が表示される


        // 選択したグループのコマンドを選択不可にする

    }

    public void ResetActionState() {
        ActionState.Value = UnitActionState.NotActed;
    }

    public BattlePreviewData CreatePreview(SkillData skillData, CharaUnit target = null) {
        if (target == null) {
            //// 自己対象スキル（回復・バフなど）
            //return new BattlePreviewData {
            //    attacker = this,
            //    skillUsed = skill,
            //    damage = 0,
            //    hit = 100,
            //    crit = 0,
            //    isCounter = false
            //};

            return null;
        }

        if (skillData == null) {
            skillData = new SkillData();
            skillData.value = 0;
            skillData.accuracy = 0;
            skillData.critical = 0;
        }

        int damage = Mathf.Max(charaData.attack + skillData.value - target.charaData.defense, 0);
        int hit = Mathf.Clamp(skillData.accuracy + charaData.technic - target.charaData.evasion, 0, 100);
        int crit = Mathf.Clamp(skillData.critical - target.charaData.luck, 0, 100);

        return new BattlePreviewData {
            attacker = this,
            defender = target,
            skillUsed = skillData,
            damage = damage,
            hit = hit,
            crit = crit,
            isCounter = false
        };
    }

    /// <summary>
    /// 選択中のスキルを取得して戻す
    /// </summary>
    /// <returns></returns>
    public SkillData GetSelectSkillData() {
        return DataBaseManager.instance.GetSkillData(selectSkillId);
    }

    public BattleAnimator GetBattleAnimator() {
        return battleAnimator;
    }

    /// <summary>
    /// ユニットの現在の位置情報(ワールド座標)を、タイルマップの座標(セル座標)に変換
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    public Vector3Int GetGridPosition(Tilemap tilemap) {
        return tilemap.WorldToCell(transform.position);
    }
}