using UI;

public class UIButtonPiggyBank : UIBaseButton
{
    public async override void Action()
    {
        var lastUi = UI.PanelManager.Instance.GetLast();
        var panel = await PanelManager.CreateAsync<UIPiggyBankPanel>(AddressableName.UIPiggyBankPanel);
        await lastUi.HideByTransitions();
        panel.Show();
        panel.onClosed += lastUi.ShowByTransitions;
    }
}
