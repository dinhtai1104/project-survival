using UnityEngine;

public class UIMergeRarityResultPanel : UI.Panel
{
    [SerializeField] private UIInventorySlot slot;
    [SerializeField] private UIMergeRarityInforNewItemView uiInfor;
    public override void PostInit()
    {

    }

    public async void Show(EquipableItem item)
    {
        base.Show();
        var equipData = new EquipmentData(item.Id, item.EquipmentRarity, item.ItemLevel);
        var eq = await UIHelper.GetUILootIcon(AddressableName.UIGeneralEquipmentItem, equipData, slot.transform);
        slot.SetItem(eq);

        uiInfor.Set(item);

        onClosed += () =>
        {
            slot.Clear();
        };
    }
}
