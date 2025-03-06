using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public class CommandGenerator : AbstractSingleton<CommandGenerator> {

    [SerializeField] private CommandDataSO commandDataSO;

    private Dictionary<string, IObjectPool<ICommand>> commandPools = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        InitializeCommandPools();
    }

    /// <summary>
    /// 各コマンドのオブジェクトプールを初期化
    /// </summary>
    public void InitializeCommandPools() {
        foreach (CommandData commandData in commandDataSO.commandDataList) {
            if (commandData == null) continue;
       
            string commandName = commandData.name;
            IObjectPool<ICommand> pool = CreateObjectPool(commandData);

            //commandPools[commandName] = pool;
            commandPools.Add(commandName, pool);
        }
    }

    private IObjectPool<ICommand> CreateObjectPool(CommandData commandData) {
        return new ObjectPool<ICommand>(
            createFunc: () => CreateCommand(commandData),
            actionOnGet: command => Debug.Log($"Command {commandData.name} retrieved."),
            actionOnRelease: command => command.Release()
            );


    }

    private ICommand CreateCommand(CommandData commandData) {
        Type type = Type.GetType(commandData.name);
        if(type == null || typeof(ICommand).IsAssignableFrom(type)) {
            return null;
        }

        return Activator.CreateInstance(type) as ICommand;
    }


    public ICommand GetCommand(string commandName, params object[] args) {
        if (commandPools.TryGetValue(commandName, out IObjectPool<ICommand> pool)) {
            ICommand command = pool.Get();
            command.Initialize(args);
            return command;
        }
        throw new KeyNotFoundException($"Command pool for {commandName} not found.");


    }
}