using UnityEngine;
using R3;
using System.Collections.Generic;

public enum CommandStateType {
    CursorMove,   // カーソル移動可能状態
    CommandUI,    // コマンド UI 表示中（カーソル停止）
    UnitAction,   // ユニットの移動中やアクション中
    UnitMove,
    Inactive,     // プレイヤーのキー入力を受け付けない。敵のターン、ムービー再生中など。
    TargetSelect,
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
    [SerializeField] private UICommandManager uiCommandManager;
    [SerializeField] private UnitSelection unitSelection;


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

        uiCommandManager.SetupUICommandManager();

        // GameState を購読して、カーソル移動を開始
        CommandState.Where(state => state == CommandStateType.CursorMove)
            .Subscribe(_ => {
                cameraController.ResetCamera();
                moveRangeVisualizer.ClearHighlight();
                uiCommandManager.DisableCommandUI();
            })
            .AddTo(disposables);

        // GameState を購読して UI 操作を開始
        CommandState.Where(state => state == CommandStateType.CommandUI)
            .Subscribe(_ => uiCommandManager.EnableCommandUI())
            .AddTo(this);


        CommandState.Where(state => state == CommandStateType.UnitMove)
            .Subscribe(_ => { 
                uiCommandManager.DisableCommandUI();
                unitSelection.HighlightMoveRange();
            })
            .AddTo(this);


        CommandState.Where(state => state == CommandStateType.TargetSelect)
            .Subscribe(_ => {
                uiCommandManager.DisableCommandUI();
                unitSelection.HighlightAttackRange();
            }).AddTo(this);


        // Command 系の処理
        //commandInvoker = new();   // static メソッドは、クラスインスタンス不要

        // ゲームのたびに static 情報を初期化
        CommandInvoker.ClearStacks();
    }

    private void OnPlayerTurnEnd() {
        Debug.Log("Player Turn End");
        ChangeState(CommandStateType.Inactive);
        StartEnemyTurn();
    }

    private void OnEnemyTurnEnd() {
        Debug.Log("Enemy Turn End");
        ChangeState(CommandStateType.CursorMove);
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