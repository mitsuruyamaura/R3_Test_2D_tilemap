[System.Serializable]
public abstract class CommandDetailDataBase {
    public string label;
    //public UnityAction onClickAction;

    //public CommandDetailData(string label, UnityAction action) {
    //    this.label = label;
    //    this.onClickAction = action;
    //}

    protected CommandDetailDataBase(string label) {
        this.label = label;
    }

    public void Execute(CharaUnit charaUnit) { }
}