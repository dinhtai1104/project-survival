using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIButtonStarterPack : UIBaseButton
{
    public override void Action()
    {
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIFlashSaleStarterPackPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
