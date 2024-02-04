public class UIDebugButtonShowStat : UIBaseButton
{
    public async override void Action()
    {
        (await UI.PanelManager.CreateAsync<UIDebugStatPanel>(AddressableName.UIDebugStatPanel)).Show();
    }
}