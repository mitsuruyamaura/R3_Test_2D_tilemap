using System.Linq;
using UnityEngine;

public class DataBaseManager : AbstractSingleton<DataBaseManager> {
    [SerializeField] private SkillDataSO skillDataSO;
    [SerializeField] private CharaDataSO charaDataSO;

    public SkillData GetSkillData(int searchSkillID) {
        return skillDataSO.skillDataList.FirstOrDefault(data => data.skillId == searchSkillID);
    }

    public CharaData GetCharaData(int searchCharaDataID) {
        return charaDataSO.charaDataList.FirstOrDefault(data => data.charaId == searchCharaDataID);
    }
}