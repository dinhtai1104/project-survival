using UnityEngine;

public class UIMergeRarityPanel : UI.Panel
{
    [SerializeField] private UIInventoryAllEquipementMergeRarityView inventoryView;
    private InventorySave inventorySave;
    public override void PostInit()
    {
        inventorySave = DataManager.Save.Inventory;
    }
    public override void Show()
    {
        base.Show();
        ShowInventory();
    }
    public void ShowInventory()
    {
        inventoryView.Show();
    }
    public override void Close()
    {
        inventoryView.Clear();
        base.Close();
    }
}
