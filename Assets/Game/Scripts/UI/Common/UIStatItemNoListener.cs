public class UIStatItemNoListener : UIStatItem
{
    protected override void OnDisable()
    {
    }
    protected override void OnEnable()
    {
    }
    public override void SetAddStat(float data)
    {
        base.SetAddStat(data);
        if (statAddTxt == null) return;
        if (data == 0)
        {
            statAddTxt.text = "";
            return;
        }
        statAddTxt.text = $"<color=white>(</color>+{data.TruncateValue()}<color=white>)</color>";
    }
}