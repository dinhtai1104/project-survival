using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UI;

public class UIButtonDungeonEvent : UIBaseButton
{
    public override async void Action()
    {
        var last = UI.PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIDungeonEventPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
