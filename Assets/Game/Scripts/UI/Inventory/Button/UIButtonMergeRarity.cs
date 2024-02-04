using UI;

public class UIButtonMergeRarity : UIBaseButton
{
    public override async void Action()
    {
        var last = PanelManager.Instance.GetLast();
        var panel = await PanelManager.CreateAsync(AddressableName.UIMergeRarityPanel);
        await last.HideByTransitions();
        panel.Show();
        panel.onClosed += last.ShowByTransitions;
    }
}