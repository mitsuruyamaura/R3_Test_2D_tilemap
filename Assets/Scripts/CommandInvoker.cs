﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker {
    private static Stack<ICommand> undoStack = new();
    private static Stack<ICommand> redoStack = new();


    /// <summary>
    /// コマンドを実行し、undoStackに追加
    /// </summary>
    /// <param name="command"></param>
    public static void ExecuteCommand(ICommand command) {
        command.Execute();
        undoStack.Push(command);

        redoStack.Clear();
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
    public void RedoCommand() {
        if(redoStack.Count > 0) {
            ICommand activeCommand = redoStack.Pop();
            undoStack.Push(activeCommand);

            activeCommand.Execute();
        }
    }
}
