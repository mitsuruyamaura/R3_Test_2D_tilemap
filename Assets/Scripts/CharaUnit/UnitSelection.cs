using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using System.Reactive.Linq;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using System.Collections;

/// <summary>
/// ユニット選択
/// </summary>
public class UnitSelection : MonoBehaviour {
    public CursorController cursorController; // カーソルの管理
    public TilemapController tilemapController; // タイルマップの移動範囲計算
    public MoveRangeVisualizer moveRangeVisualizer; // 移動範囲表示
    public CameraController cameraController;
    public GameManager gameManager;
    public BattleUI battleUI;

    private CharaUnit selectedUnit;
    private CharaUnit targetUnit;
    private Subject<(Unit, bool)> buttonSubject = new();
    private BattleSysyem battleSysyem;
    private CancellationTokenSource cts;

    [SerializeField] private BattleSystemCoroutineBase battleSystemCoroutine;
    private bool? decision = null;

    void Start() {
        cts = new();
        battleSysyem = new(battleUI);

        //        battlePreviewUI.btnSubmit.OnClickAsObservable()
        //.ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
        //.Subscribe(_ => buttonSubject.OnNext((default, true))).AddTo(this);

        battleUI.Setup();

        battleUI.OnClickDecision
            .Subscribe(isConfirmed => {
                if (isConfirmed) {
                    buttonSubject.OnNext((default, true));
                } else {
                    buttonSubject.OnNext((default, false));
                }
            }).AddTo(this);

        if (battleSystemCoroutine != null) {
            battleUI.SetCallbacksForBattleSystemCoroutine(
                onSubmit: () => decision = true,
                onCancel: () => decision = false
            );
        }

    }

    private void Update() {
        // キー入力でユニットを選択
        if (Input.GetKeyDown(KeyCode.Return) && gameManager.CommandState.Value == CommandStateType.CursorMove) {
            SelectUnitAtCursorPositionAsync().Forget();
        }

        // キー入力で移動先を選択
        if (selectedUnit != null && Input.GetKeyDown(KeyCode.Space) && gameManager.CommandState.Value == CommandStateType.UnitMove) {
            Vector3Int cursorPosition = cursorController.GetCursorPosition();
            TryMoveUnit(cursorPosition);
        }

        // UI コマンドが開いている場合、閉じる
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CommandState.Value == CommandStateType.CommandUI) {
            // 移動範囲のハイライトをクリア
            //moveRangeVisualizer.ClearHighlight();

            // UI コマンドを非表示して、タイルのカーソル移動状態に戻す
            gameManager.ChangeState(CommandStateType.CursorMove);
        }

        // 移動範囲表示中の場合
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CommandState.Value == CommandStateType.UnitMove) {
            // 移動範囲のハイライトをクリア
            moveRangeVisualizer.ClearHighlight();

            // UI コマンドを再表示
            gameManager.ChangeState(CommandStateType.CommandUI);
        }

        // 攻撃範囲表示中の場合
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.CommandState.Value == CommandStateType.TargetSelect) {
            // 攻撃範囲のハイライトをクリア
            moveRangeVisualizer.ClearHighlight();

            // UI コマンドを再表示
            gameManager.ChangeState(CommandStateType.CommandUI);
        }
    }

    public async UniTask SelectUnitAtCursorPositionAsync() {
        Vector3Int cursorPosition = cursorController.GetCursorPosition();
        CharaUnit unitAtPosition = GetUnitAtPosition(cursorPosition);

        if (unitAtPosition != null) {
            selectedUnit = unitAtPosition; // ユニットを選択
            //HighlightMoveRange(selectedUnit); // 移動範囲を表示
            cameraController.FocusOnUnit(selectedUnit.transform);
            
            await UniTask.Yield();

            // UI(コマンド、キャラステータス)表示
            gameManager.ChangeState(CommandStateType.CommandUI);

            Debug.Log($"ユニット選択 : {selectedUnit.charaData}");
        } else {
            Debug.Log("カーソル位置にユニットがいません");
        }
    }


    public async UniTask TargetUnitAtCursonPositionAsync() {
        gameManager.ChangeState(CommandStateType.UnitAction);

        Vector3Int cursorPosition = cursorController.GetCursorPosition();
        CharaUnit unitAtPosition = GetUnitAtPosition(cursorPosition);

        if (unitAtPosition != null) {
            targetUnit = unitAtPosition; // 対象となるユニットを設定
            //HighlightMoveRange(selectedUnit); // 移動範囲を表示
            cameraController.FocusOnUnit(targetUnit.transform);

            await UniTask.Yield();

            // 双方のキャラの情報を利用して、ダメージなどの計算を行う
            // 攻撃側
            SkillData attackerSkillData = selectedUnit.GetSelectSkillData();
            BattlePreviewData attackerPreviewData = selectedUnit.CreatePreview(attackerSkillData, targetUnit);

            // 防御側
            SkillData targerSkillData = targetUnit.GetSelectSkillData();
            BattlePreviewData targerPreviewData = targetUnit.CreatePreview(targerSkillData, selectedUnit);

            // バトルプレビュー表示
            // 決定・キャンセルのボタン表示
            battleUI.ShowBattlePreview(attackerPreviewData, targerPreviewData);
            Debug.Log($"行動対象先ユニット選択 : {targetUnit.charaData}");

            // ユーザーが「OK」か「キャンセル」ボタンを押すと OnClickDecision に bool が流れる。
            // FirstAsync() により「最初に流れてきた値（押下結果）」だけを待つ。
            // AsUniTask() によって await 可能な UniTask<bool> に変換される
            bool isConfirmed = await battleUI.OnClickDecision.FirstAsync().AsUniTask();

            // OK を押した場合
            if (isConfirmed) {
                // プレビュー表示したまま、プレビュー上でやりとりする
                await battleSysyem.ExecuteBattleAsync(attackerPreviewData, targerPreviewData, cts.Token);
            } else {
                // キャンセル時の処理

            }
            // タイルマップのハイライト解除
            moveRangeVisualizer.ClearHighlight();

            // プレビュー閉じる
            battleUI.HideBattlePreview();

            gameManager.ChangeState(CommandStateType.CursorMove);
        } else {
            Debug.Log("カーソル位置にユニットがいません");
            gameManager.ChangeState(CommandStateType.CursorMove);
        }
    }


    private CharaUnit GetUnitAtPosition(Vector3Int position) {
        // ゲーム内に配置されているすべてのユニットをチェックし、位置に対応するユニットを返す
        foreach (CharaUnit unit in FindObjectsByType<CharaUnit>(FindObjectsSortMode.None)) {
            Vector3 charaPos = unit.transform.position;
            Vector3 tilePos = tilemapController.tilemap.GetCellCenterWorld(position);
            Debug.Log($"{charaPos} : {tilePos}");
            if (charaPos == tilePos) {
                return unit;
            }
        }
        return null;
    }

    /// <summary>
    /// GameManager にて CommandState が UnitMove になると実行される
    /// </summary>
    public void HighlightMoveRange() {
        // 移動範囲を計算して表示
        Vector3Int unitPosition = tilemapController.tilemap.WorldToCell(selectedUnit.transform.position);
        HashSet<Vector3Int> moveRange = tilemapController.CalculateMoveRange(unitPosition, selectedUnit.charaData.movePower);
        moveRangeVisualizer.HighlightMoveRange(moveRange);
    }

    /// <summary>
    /// GameManager にて CommandState が TargetUnit になると実行される
    /// </summary>
    public void HighlightAttackRange() {
        Vector3Int unitPosition = tilemapController.tilemap.WorldToCell(selectedUnit.transform.position);
        HashSet<Vector3Int> attackRange = tilemapController.CalculateRange(unitPosition, selectedUnit.charaData.range);
        moveRangeVisualizer.HighlightAttackRange(attackRange);
    }

    private void TryMoveUnit(Vector3Int targetPosition) {
        if (selectedUnit != null && tilemapController.CalculateMoveRange(tilemapController.tilemap.WorldToCell(selectedUnit.transform.position), selectedUnit.charaData.movePower).Contains(targetPosition)) {
            
            // 初心者向けはこっち
            MoveUnitToTile(selectedUnit, targetPosition);

            // コマンドパターンの場合には、ここで MoveCommand を生成して Invoker に渡す


        } else {
            Debug.Log("移動範囲外です");
        }
    }

    private void MoveUnitToTile(CharaUnit unit, Vector3Int targetPosition) {
        unit.transform.position = tilemapController.tilemap.GetCellCenterWorld(targetPosition);
        Debug.Log("ユニットが移動しました");

        // 移動完了後、移動範囲をクリア
        moveRangeVisualizer.ClearHighlight();

        // カーソルを消して、UI 移動に戻す
        gameManager.ChangeState(CommandStateType.CommandUI);

    }


    public CharaUnit GetSelectedUnit() {
        return selectedUnit;
    }


    public IEnumerator TargetUnitAtCursonPositionCoroutine() {
        decision = null;
        gameManager.ChangeState(CommandStateType.UnitAction);

        Vector3Int cursorPosition = cursorController.GetCursorPosition();
        CharaUnit unitAtPosition = GetUnitAtPosition(cursorPosition);

        if (unitAtPosition != null) {
            targetUnit = unitAtPosition; // 対象となるユニットを設定
            //HighlightMoveRange(selectedUnit); // 移動範囲を表示
            cameraController.FocusOnUnit(targetUnit.transform);

            yield return null;

            // 双方のキャラの情報を利用して、ダメージなどの計算を行う
            // 攻撃側
            SkillData attackerSkillData = selectedUnit.GetSelectSkillData();
            BattlePreviewData attackerPreviewData = selectedUnit.CreatePreview(attackerSkillData, targetUnit);

            // 防御側
            SkillData targerSkillData = targetUnit.GetSelectSkillData();
            BattlePreviewData targerPreviewData = targetUnit.CreatePreview(targerSkillData, selectedUnit);

            // バトルプレビュー表示
            // 決定・キャンセルのボタン表示
            battleUI.ShowBattlePreview(attackerPreviewData, targerPreviewData);
            Debug.Log($"行動対象先ユニット選択 : {targetUnit.charaData}");

            // 実行かキャンセルのいずれかのボタンを押してフラグが決まるまで待機
            yield return new WaitUntil(() => decision.HasValue);

            // 実行を押した場合
            if (decision.Value == true) {
                // プレビュー表示したまま、プレビュー上でやりとりする
                yield return battleSystemCoroutine.ExecuteBattleCo(attackerPreviewData, targerPreviewData);
            } else {
                // キャンセル時の処理

            }

            // タイルマップのハイライト解除
            moveRangeVisualizer.ClearHighlight();

            // プレビュー閉じる
            battleUI.HideBattlePreview();

            gameManager.ChangeState(CommandStateType.CursorMove);
        } else {
            Debug.Log("カーソル位置にユニットがいません");
            gameManager.ChangeState(CommandStateType.CursorMove);
        }
    }
}