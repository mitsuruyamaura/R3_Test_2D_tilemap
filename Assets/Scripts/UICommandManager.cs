using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System.Linq;

public class UICommandManager : MonoBehaviour {

    [SerializeField] private CommandGroup[] commandGroups;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int currentGroupIndex = 0;
    [SerializeField] private int currentDetailIndex = 0;

    void Start() {
        // 初期状態: 最初の CommandGroup とその最初の CommandDetail を選択
        HighlightGroup(currentGroupIndex);
        HighlightDetail(currentGroupIndex, currentDetailIndex);

        commandGroups.ToList().ForEach(group => group.SetUpCommandGroup(ExecuteCommandProc, gameManager.CommandState));

        // GameState を購読して UI 操作を開始
        gameManager.CommandState
            .Where(state => state == CommandStateType.CommandUI)
            .Subscribe(_ => EnableCommandUI())
            .AddTo(this);

        gameManager.CommandState
            .Where(state => state == CommandStateType.CursorMove)
            .Subscribe(_ => DisableCommandUI())
            .AddTo(this);
    }

    private void EnableCommandUI() {
        Debug.Log("コマンド UI 表示開始");
        SetDefaultGroupAndDetail();

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    private void DisableCommandUI() {
        Debug.Log("コマンド UI 表示終了");
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
    }

    void Update() {
        if (gameManager.CommandState.Value != CommandStateType.CommandUI)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveGroup(-1);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveGroup(1);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveDetail(-1);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveDetail(1);
        }
    }

    /// <summary>
    /// Commandグループと Detail を初期値に戻す
    /// </summary>
    private void SetDefaultGroupAndDetail() {
        // 現在のグループのハイライトを解除
        commandGroups[currentGroupIndex].Unhighlight();

        // グループのインデックスを更新
        currentGroupIndex = 0;

        // 新しいグループをハイライトし、その最初のコマンドを選択
        HighlightGroup(currentGroupIndex);
        currentDetailIndex = 0;
        HighlightDetail(currentGroupIndex, currentDetailIndex);
    }

    private void MoveGroup(int direction) {
        // 現在のグループのハイライトを解除
        commandGroups[currentGroupIndex].Unhighlight();

        // グループのインデックスを更新
        currentGroupIndex = (currentGroupIndex + direction + commandGroups.Length) % commandGroups.Length;

        // 新しいグループをハイライトし、その最初のコマンドを選択
        HighlightGroup(currentGroupIndex);
        currentDetailIndex = 0;
        HighlightDetail(currentGroupIndex, currentDetailIndex);
    }

    private void MoveDetail(int direction) {
        var commandDetails = commandGroups[currentGroupIndex].CommandDetailList;

        // 現在のコマンド詳細のハイライトを解除
        commandDetails[currentDetailIndex].Unhighlight();

        // コマンド詳細のインデックスを更新
        currentDetailIndex = (currentDetailIndex + direction + commandDetails.Count) % commandDetails.Count;

        // 新しいコマンド詳細をハイライト
        HighlightDetail(currentGroupIndex, currentDetailIndex);
    }

    private void HighlightGroup(int groupIndex) {
        commandGroups[groupIndex].Highlight();
        Debug.Log($"CommandGroup {groupIndex} をハイライトしました。");
    }

    private void HighlightDetail(int groupIndex, int detailIndex) {
        commandGroups[groupIndex].CommandDetailList[detailIndex].Highlight();
        Debug.Log($"CommandDetail {detailIndex} をハイライトしました。");
    }

    public void ExecuteCommandProc() {        
        gameManager.ChangeState(CommandStateType.CursorMove);
        Debug.Log("ExecuteCommandProc");
    }
}