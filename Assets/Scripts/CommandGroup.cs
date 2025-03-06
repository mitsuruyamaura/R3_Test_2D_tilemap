using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Events;
using R3;

public class CommandGroup : MonoBehaviour {

    [SerializeField] private List<CommandDetail> commandDetailList;

    public List<CommandDetail> CommandDetailList => commandDetailList;


    public void SetUpCommandGroup(UnityAction onClickCallback, SerializableReactiveProperty<CommandStateType> CommandState) {
        commandDetailList.ForEach(detail => detail.SetUpCommandDetail(onClickCallback, CommandState));
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
}