using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Create SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    public List<SkillData> skillDataList = new();

    public SkillData GetSkillData(int searchId) {
        return skillDataList.FirstOrDefault(data => data.skillId == searchId);
    }
}