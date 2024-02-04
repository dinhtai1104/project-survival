using UnityEngine;

public class UIButtonCollapseSettings : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Settings Game");
        var uiNotice = await UI.PanelManager.CreateAsync<UICollapseSettingPanel>(AddressableName.UICollapseSettingPanel);
        uiNotice.Show();
    }
}
