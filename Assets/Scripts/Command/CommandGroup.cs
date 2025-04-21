using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using R3;


public enum CommandGroupType {
    Wait,
    Action,
    SubAction,
    Move
}


public class CommandGroup : MonoBehaviour {

    [SerializeField] private List<CommandDetail> commandDetailList;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CommandGroupType commandGroupType;
    public CommandGroupType CommandGroupType => commandGroupType;

    public List<CommandDetail> CommandDetailList => commandDetailList;


    public void SetUpCommandGroup(UnityAction<CommandGroupType> onClickCallback, SerializableReactiveProperty<CommandStateType> CommandState) {
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));
    }

    public void SetUpCommandGroup(SerializableReactiveProperty<CommandStateType> CommandState) {
        UnityAction<CommandGroupType> onClickCallback = null;

        switch (commandGroupType) {
            case CommandGroupType.Move:
                onClickCallback = (groupType) => Debug.Log("Move 処理が呼ばれました！");
                break;
            case CommandGroupType.Action:
                onClickCallback = (groupType) => Debug.Log("Action 処理が呼ばれました！");
                break;
            case CommandGroupType.Wait:
                onClickCallback = (groupType) => Debug.Log("Wait 処理が呼ばれました！");
                break;
        }

        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));

    }


    public void SetUpCommandGroups(SerializableReactiveProperty<CommandStateType> CommandState) {
        switch (commandGroupType) {
            case CommandGroupType.Move:
                SetMoveCommands(CommandState);
                break;

            case CommandGroupType.Action:
                SetActionCommands(CommandState);
                break;

            case CommandGroupType.Wait:
                SetWaitCommands(CommandState);
                break;

            case CommandGroupType.SubAction:
                SetSubActionCommands(CommandState);
                break;
        }
    }

    private void SetMoveCommands(SerializableReactiveProperty<CommandStateType> CommandState) {
        UnityAction<CommandGroupType> onClickCallback = OnMoveCommandClicked;
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));
    }

    private void SetActionCommands(SerializableReactiveProperty<CommandStateType> CommandState) {
        UnityAction<CommandGroupType> onClickCallback = OnActionCommandClicked;
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));
    }

    private void SetWaitCommands(SerializableReactiveProperty<CommandStateType> CommandState) {
        UnityAction<CommandGroupType> onClickCallback = OnWaitCommandClicked;
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));
    }

    private void SetSubActionCommands(SerializableReactiveProperty<CommandStateType> CommandState) {
        UnityAction<CommandGroupType> onClickCallback = OnSubActionCommandClicked;
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState, commandGroupType));
    }

    // 実際の処理はここ
    private void OnMoveCommandClicked(CommandGroupType groupType) {
        Debug.Log("移動コマンドが選択されました。");
        // 実際の移動処理に切り替えたり、状態を変更したりする
    }

    private void OnActionCommandClicked(CommandGroupType groupType) {
        Debug.Log("アクションコマンドが選択されました。");
    }

    private void OnWaitCommandClicked(CommandGroupType groupType) {
        Debug.Log("待機コマンドが選択されました。");
    }

    private void OnSubActionCommandClicked(CommandGroupType groupType) {
        Debug.Log("サブアクションコマンドが選択されました。");
    }


    public void Highlight() {
        // グループをハイライトする処理
        Debug.Log($"{gameObject.name} が選択されました。");
    }

    public void Unhighlight() {
        // グループ内の全ボタンのハイライトを解除する処理
        commandDetailList.ForEach(detail => detail.Unhighlight());
        Debug.Log($"{gameObject.name} の選択が解除されました。");
    }


    public void Inactivate() {
        canvasGroup.blocksRaycasts = false;
        commandDetailList.ForEach(detail => detail.Inactivate());
        Debug.Log($"{gameObject.name} のボタンが非アクティブになりました。");
    }


    public void Activate() {
        canvasGroup.blocksRaycasts = true;
        commandDetailList.ForEach(detail => detail.Activate());
        Debug.Log($"{gameObject.name} のボタンがアクティブになりました。");
    }
}