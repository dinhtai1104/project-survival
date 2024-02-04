using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIButtonQuest : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Quest Game");
        var last = PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIQuestPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();
    }
}
