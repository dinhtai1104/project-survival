public class UIButtonShowActivatedBuff : UIBaseButton
{
    public override async void Action()
    {
        var ui = await UI.PanelManager.CreateAsync<UIActivedBuffsPanel>(AddressableName.UIActivedBuffsPanel);
        ui.Show();
    }
}
