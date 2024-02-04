using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UIButtonLoadout : UIBaseButton
{
    public override async void Action()
    {
        Debug.Log("Show Loadout Game");
        var last = UI.PanelManager.Instance.GetLast();
        last.HideByTransitions().Forget();

        PanelManager.CreateAsync(AddressableName.UIInventoryPanel).ContinueWith(panel =>
        {
            panel.Show();
            panel.onClosed += last.ShowByTransitions;
        }).Forget();

    }
}
