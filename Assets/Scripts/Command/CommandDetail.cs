using R3;
using R3.Triggers;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CommandType {
    Move,
    Attack,
    Wait,
    Magic,
    Item
}

public class CommandDetail : MonoBehaviour {
    [SerializeField] private Button btnCommand;
    private IDisposable enterKeySubscription;

    public void SetUpCommandDetail(UnityAction<CommandGroupType> onClickCallback, SerializableReactiveProperty<CommandStateType> CommandState, CommandGroupType groupType) {
        btnCommand.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
            .SubscribeAwait(async (_, ct) => {
                onClickCallback?.Invoke(groupType);
                await CommandHandler.ExecuteCommandAsync();
            }, AwaitOperation.Drop)
            .RegisterTo(destroyCancellationToken);

        //// CommandState監視によるEnterキーの制御
        //CommandState
        //    .DistinctUntilChanged() // ステート変更時にだけ反応
        //    .Subscribe(newState => {
        //        HandleEnterKey(newState, onClickCallback);
        //    })
        //    .AddTo(this);

        // ステートの切り替えをフレームをまたいでから行うことで、同一キーの連続入力を回避している
        // そのため、ユニットの選択とコマンドの選択の両方で Enter キーで対応可能
        this.UpdateAsObservable()
            .Where(_ => CommandState.Value == CommandStateType.CommandUI)
            .Where(_ => Input.GetKeyDown(KeyCode.Return))
            .ThrottleFirst(System.TimeSpan.FromSeconds(2.0f))
            .SubscribeAwait(async (_, ct) => {
                onClickCallback?.Invoke(groupType);
                await CommandHandler.ExecuteCommandAsync();
            }, AwaitOperation.Drop)
            .RegisterTo(destroyCancellationToken);

        //// こちらでもOK
        //// CommandState監視によるEnterキーの制御
        //CommandState
        //    .DistinctUntilChanged() // ステート変更時にだけ反応
        //    .SelectMany(newState => {
        //        if (newState == CommandStateType.CommandUI) {
        //            // CommandUI ステートの間だけ Enter キーを監視
        //            return this.UpdateAsObservable()
        //            .Where(_ => newState == CommandStateType.CommandUI)
        //                .Where(_ => Input.GetKeyDown(KeyCode.Return))
        //                .ThrottleFirst(TimeSpan.FromSeconds(2.0f)) // 重複防止
        //                .Select(_ => newState); // 現在のステートを流す
        //        } else {
        //            // 他のステートでは何もせず空のストリームを返す
        //            return Observable.Empty<CommandStateType>();
        //        }
        //    })
        //    .Subscribe(async state => {
        //        // Enter キーが押されたときの処理
        //        onClickCallback?.Invoke();
        //        await CommandHandler.ExecuteCommandAsync();
        //    })
        //    .AddTo(this);
    }

    private void HandleEnterKey(CommandStateType state, UnityAction onClickCallback) {
        // 既存の購読を破棄して再設定
        enterKeySubscription?.Dispose();

        enterKeySubscription = this.UpdateAsObservable()
            .Where(_ => state == CommandStateType.CommandUI)
            .Where(_ => Input.GetKeyDown(KeyCode.Return)) // Enterキーを検知
            .ThrottleFirst(TimeSpan.FromSeconds(2.0f)) // 重複防止
            .SubscribeAwait(async (_, ct) => {
                onClickCallback?.Invoke();
                await CommandHandler.ExecuteCommandAsync();
            }, AwaitOperation.Drop)
            .RegisterTo(destroyCancellationToken);
    }

    public void Highlight() {
        // ボタンをハイライトする処理
        var colors = btnCommand.colors;
        colors.normalColor = Color.yellow;
        btnCommand.colors = colors;
    }

    public void Unhighlight() {
        // ボタンのハイライトを解除する処理
        var colors = btnCommand.colors;
        colors.normalColor = Color.white;
        btnCommand.colors = colors;
    }


    public void Inactivate() {
        btnCommand.interactable = false;
    }


    public void Activate() {
        btnCommand.interactable = true;
    }
}