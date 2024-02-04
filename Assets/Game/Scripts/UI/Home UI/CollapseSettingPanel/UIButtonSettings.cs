using UnityEngine;

public class UIButtonSettings : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Click settings");
        var panel = await UI.PanelManager.CreateAsync<UISettingPanel>(AddressableName.UISettingPanel);
        panel.Show();
    }
}
