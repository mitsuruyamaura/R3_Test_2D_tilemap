using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandDataSO", menuName = "Create CommandDataSO")]
public class CommandDataSO : ScriptableObject {
    public List<CommandData> commandDataList = new();
}