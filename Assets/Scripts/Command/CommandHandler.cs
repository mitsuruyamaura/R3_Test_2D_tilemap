using UnityEngine;
using Cysharp.Threading.Tasks;

public static class CommandHandler {

    public static async UniTask ExecuteCommandAsync() {
        await UniTask.Delay(1000);
        Debug.Log("Command Executed");
    }
}