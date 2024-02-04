using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;
using UI;
using UI.Tooltip;

public class InventoryTooltipItem : UIBaseTooltip
{
    private UIInventorySlot inventorySlot;

    public UIInventorySlot InventorySlot
    {
        get
        {
            if (inventorySlot == null) inventorySlot = GetComponent<UIInventorySlot>();
            return inventorySlot;
        }
    }

    public override void ShowTooltip()
    {
        var uiEquipment = InventorySlot.Get<UIGeneralEquipmentIcon>();
        if (uiEquipment != null)
        {
            var data = uiEquipment.equimentData;
            if (data.IsRandom)
            {
                PanelManager.CreateAsync<UITooltipPanel>(AddressableName.UITooltipPanel).ContinueWith(t =>
                {
                    t.Show(data);
                }).Forget();
            }
            else
            {
                var getLastUI = PanelManager.Instance.GetLast().GetType();
                if (getLastUI == typeof(UIInventoryPanel) || getLastUI == typeof(UIMergeRarityPanel)) return;
                var equi = GameSceneManager.Instance.EquipmentFactory.CreateEquipment(data);
                var save = equi.EquipmentSave;
                PanelManager.CreateAsync<UIEquipmentInforPanel>(AddressableName.UITooltipEquipmentInforPanel).ContinueWith(t =>
                {
                    t.Show(equi, true);
                }).Forget();
            }
            return;
        }

        var uiResource = InventorySlot.Get<UIGeneralBaseIcon>();
        if (uiResource != null && !uiEquipment)
        {
            var loot = uiResource.GetComponent<UILootItemBase>();
            var data = loot.Data;
            if (data != null)
            {
                PanelManager.CreateAsync<UITooltipPanel>(AddressableName.UITooltipPanel).ContinueWith(t =>
                {
                    t.Show(data);
                }).Forget();
            }
        }
    }
}
