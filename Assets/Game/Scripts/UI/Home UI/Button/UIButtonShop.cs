using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIButtonShop : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Shop Game");
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIShopPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
