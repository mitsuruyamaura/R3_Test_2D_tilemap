using UnityEngine;

/// <summary>
/// 移動コマンドを実行して、指定されたユニットを動かす
/// </summary>
[System.Serializable]
public class MoveCommand : ICommand {
    private CharaUnit charaUnit; // 移動対象のユニット
    private Vector3Int movePos; // 移動先の位置
    private Vector3Int originalPos; // 元の位置を記憶する


    public MoveCommand(CharaUnit charaUnit, Vector3Int movePos, Vector3Int originalPos) {
        this.charaUnit = charaUnit;
        this.movePos = movePos;
        this.originalPos = originalPos;
    }


    public void Execute() {
        if (charaUnit == null) {
            Debug.LogError("CharaUnit が null です。");
            return;
        }


        //ICommand command = new MoveCommand(null, new(0, 0, 0));
        //CommandInvoker.ExecuteCommand(command);

        // TODO CommandStateType を UnitMove に切り替える


        // カーソルの移動をマップに移し、コマンド UI を消す


        // Escape キーで 移動範囲を消して、コマンド再表示。カーソルを UI 移動に移す


        // 移動可能範囲で Enter キーで、移動

        // ユニットを指定した位置に移動
        //charaUnit.Move(movePos);

    }


    public void Undo() {
        // ユニットを元の位置に戻す
        charaUnit.Move(-movePos);
    }


    public void Release() {
        throw new System.NotImplementedException();
    }


    public void Initialize(params object[] args) {
        throw new System.NotImplementedException();
    }
}