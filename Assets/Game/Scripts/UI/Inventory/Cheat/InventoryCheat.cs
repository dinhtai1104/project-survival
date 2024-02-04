using System;
using UnityEngine;

public class InventoryCheat : UICheat
{
#if DEVELOPMENT
    [SerializeField] private Transform holder;
    [SerializeField] private UIButtonCheat cheatBtn;
    protected override void Init()
    {
        base.Init();
        CreateButtonCheat("Add Item", OnAddItem);
        CreateButtonCheat("Add AllItem", OnAddAllItem);
    }

    private async void OnAddAllItem()
    {
        var ui = await UI.PanelManager.CreateAsync<UICheatAllItemPanel>(AddressableName.UICheatAllItemPanel);
        ui.Show();
    }

    private async void OnAddItem()
    {
        var ui = await UI.PanelManager.CreateAsync<UICheatAddItemPanel>(AddressableName.UICheatAddItemPanel);
        ui.Show();
    }

    private void CreateButtonCheat(string cheatName, System.Action cb)
    {
        var btn = PoolManager.Instance.Spawn(cheatBtn, holder);
        btn.SetCheat(cheatName, cb);
    }
#endif
}