using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIButtonPlayGift : UIBaseButton
{
    public override void Action()
    {
        Debug.Log("Show Play Gift Game");
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIPlayGiftPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
