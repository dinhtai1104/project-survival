using UnityEngine;

public class UIButtonEvents : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Click events");
        var uiNotice = await UI.PanelManager.CreateAsync<UINoticePanel>(AddressableName.UINoticePanel);
        uiNotice.SetText("UI Events here");
        uiNotice.Show();
    }
}
