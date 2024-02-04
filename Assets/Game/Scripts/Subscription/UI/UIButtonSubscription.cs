using Cysharp.Threading.Tasks;
using UI;

public class UIButtonSubscription : UIBaseButton
{
    public async override void Action()
    {
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UISubscriptionPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
