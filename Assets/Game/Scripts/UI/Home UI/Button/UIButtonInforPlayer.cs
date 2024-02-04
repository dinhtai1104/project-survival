using UnityEngine;
public class UIButtonInforPlayer : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Infor Player Game");
        var uiNotice = await UI.PanelManager.CreateAsync<UINoticePanel>(AddressableName.UINoticePanel);
        uiNotice.SetText("UI Infor here");
        uiNotice.Show();
    }
}
