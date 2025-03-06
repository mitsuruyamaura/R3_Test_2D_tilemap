using UnityEngine;
using R3;
using System.Collections.Generic;

public enum CommandStateType {
    CursorMove,   // カーソル移動可能状態
    CommandUI,    // コマンド UI 表示中（カーソル停止）
    UnitAction    // ユニットの移動中やアクション中


}


public class GameManager : MonoBehaviour
{
    public List<CharaUnit> playerUnitList = new();
    public List<CharaUnit> enemyUnitList = new();
    public Subject<OwnerType> unitActionCompleted = new ();
    private CompositeDisposable disposables = new ();

    public SerializableReactiveProperty<CommandStateType> CommandState = new ();

    [SerializeField] private CursorController cursorController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MoveRangeVisualizer moveRangeVisualizer;

    public void ChangeState(CommandStateType newState) {
        CommandState.Value = newState;
    }

    private void Start() {
        // 各ユニットの状態を購読
        foreach (var unit in playerUnitList) {
            unit.ActionState
                .Where(state => state == UnitActionState.Acted) // 行動終了時
                .Subscribe(_ => unitActionCompleted.OnNext(unit.ownerType))
                .AddTo(disposables);
        }

        foreach (var unit in enemyUnitList) {
            unit.ActionState
                .Where(state => state == UnitActionState.Acted) // 行動終了時
                .Subscribe(_ => unitActionCompleted.OnNext(unit.ownerType))
                .AddTo(disposables);
        }

        // ターンの終了を監視
        unitActionCompleted
            .Where(_ => playerUnitList.Count > 0)
            .Where(owner => owner == OwnerType.Player)
            .Chunk(playerUnitList.Count)  // R3 から Buffer は Chunk に変更
            .Subscribe(_ => OnPlayerTurnEnd())
            .AddTo(disposables);

        unitActionCompleted
            .Where(_ => enemyUnitList.Count > 0)
            .Where(owner => owner == OwnerType.Enemy)

            .Chunk(enemyUnitList.Count)
            .Subscribe(_ => OnEnemyTurnEnd())
            .AddTo(disposables);

        // カーソルの初期化とキー入力設定
        GameObject cursor = cursorController.InitializeCursor(this);

        // カーソルのカメラ登録
        cameraController.SetDefaultCameraFollow(cursor.transform);

        CommandState.Where(state => state == CommandStateType.CursorMove)
            .Subscribe(_ => {
                cameraController.ResetCamera();
                moveRangeVisualizer.ClearHighlight();
            }).AddTo(disposables);
    }

    private void OnPlayerTurnEnd() {
        Debug.Log("Player Turn End");
        StartEnemyTurn();
    }

    private void OnEnemyTurnEnd() {
        Debug.Log("Enemy Turn End");
        StartPlayerTurn();
    }

    private void StartPlayerTurn() {
        Debug.Log("Starting Player Turn");
        foreach (var unit in playerUnitList) unit.ResetActionState();
    }

    private void StartEnemyTurn() {
        Debug.Log("Starting Enemy Turn");
        foreach (var unit in enemyUnitList) unit.ResetActionState();
    }

    private void OnDestroy() {
        disposables.Dispose();
    }
}