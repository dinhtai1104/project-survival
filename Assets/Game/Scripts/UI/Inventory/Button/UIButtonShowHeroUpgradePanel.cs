using UI;

public class UIButtonShowHeroUpgradePanel : UIBaseButton
{
    public override async void Action()
    {
        var last = PanelManager.Instance.GetLast();
        var panel = await PanelManager.CreateAsync(AddressableName.UIHeroUpgradePanel);
        await last.HideByTransitions();
        panel.Show();
        panel.onClosed += last.ShowByTransitions;
    }
}
