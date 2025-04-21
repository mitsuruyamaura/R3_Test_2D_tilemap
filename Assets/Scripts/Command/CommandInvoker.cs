using System.Collections.Generic;

public class CommandInvoker {
    private static Stack<ICommand> undoStack = new();
    private static Stack<ICommand> redoStack = new();

    /// <summary>
    /// static 情報の初期化
    /// </summary>
    public static void ClearStacks() {
        undoStack.Clear();
        redoStack.Clear();
    }

    /// <summary>
    /// コマンドを実行し、undoStackに追加
    /// </summary>
    /// <param name="command"></param>
    public static void ExecuteCommand(ICommand command) {
        command.Execute();
        undoStack.Push(command);

        redoStack.Clear();
        UnityEngine.Debug.Log($"Execute Command :{command}");
    }

    /// <summary>
    /// undoStackからコマンドを取り出し、redoStackに追加して、そのコマンドのUndo()を実行
    /// </summary>
    public static void UndoCommand() {
        if (undoStack.Count > 0) {
            ICommand activeCommand = undoStack.Pop();
            redoStack.Push(activeCommand);

            activeCommand.Undo();
        }
    }

    /// <summary>
    /// redoStackからコマンドを取り出し、undoStackに追加して、そのコマンドのExecute()を実行
    /// </summary>
    public static void RedoCommand() {
        if(redoStack.Count > 0) {
            ICommand activeCommand = redoStack.Pop();
            undoStack.Push(activeCommand);

            activeCommand.Execute();
        }
    }
}
