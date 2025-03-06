using R3;
using UnityEngine;

public enum UnitActionState {
    NotActed, // 未行動
    Acted,    // 行動済
    Unmovable // 行動不可
}

public enum OwnerType {
    Player,
    Enemy
}

public class CharaUnit : MonoBehaviour {
    public CharaData charaData;
    public ReactiveProperty<UnitActionState> ActionState = new();
    public OwnerType ownerType;


    public void Initialize(int movePower, int hp) {
        charaData = new CharaData(movePower, hp);
    }

    public void Initialize(CharaData charaData) {
        this.charaData = charaData;
    }

    public void SetActionState(UnitActionState state) {
        ActionState.Value = state;
    }

    public bool IsActionable() {
        return ActionState.Value == UnitActionState.NotActed;
    }

    public void EndAction() {
        ActionState.Value = UnitActionState.Acted;
    }

    public void ResetActionState() {
        ActionState.Value = UnitActionState.NotActed;
    }
}