using Cysharp.Threading.Tasks;
using UI;

public class UIButtonSkillTree : UIBaseButton
{
    public async override void Action()
    {
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UISkillTreePanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}