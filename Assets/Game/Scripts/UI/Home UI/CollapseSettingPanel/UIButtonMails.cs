using UnityEngine;

public class UIButtonMails : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Click mails");
        var uiNotice = await UI.PanelManager.CreateAsync<UINoticePanel>(AddressableName.UINoticePanel);
        uiNotice.SetText("UI Mails here");
        uiNotice.Show();
    }
}
