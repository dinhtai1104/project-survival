using UI;
using UnityEngine;

public class UIButtonEvolve : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Evolve Game");
        var last = PanelManager.Instance.GetLast();
        var panel = await PanelManager.CreateAsync(AddressableName.UIMergeRarityPanel);
        await last.HideByTransitions();
        panel.Show();
        panel.onClosed += last.ShowByTransitions;
    }
}
