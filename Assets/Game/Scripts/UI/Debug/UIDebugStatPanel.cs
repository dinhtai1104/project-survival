using UnityEngine;

public class UIDebugStatPanel : UI.Panel
{
    public UIDebugStatGroupItem item;
    public Transform holder;
    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        var player = GameController.Instance.GetMainActor();
        var stats = player.Stats;
        foreach (var item in stats.StatNames)
        {
            var stat = Instantiate(this.item, holder);
            stat.Setup(stats.GetStat(item), item);
            stat.gameObject.SetActive(true);
        }
    }
}
