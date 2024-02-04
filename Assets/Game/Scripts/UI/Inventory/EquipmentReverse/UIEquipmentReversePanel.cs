using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

public class UIEquipmentReversePanel : UI.Panel
{
    [SerializeField] private UIInventorySlot equipmentSlot;
    [SerializeField] private TextMeshProUGUI equipmentLevelTxt;
    [SerializeField] private UIInventorySlot equipmentSlotResult;
    [SerializeField] private Transform holderResult;
    [SerializeField] private UIIconText costPanel;
    [SerializeField] private ValueConfigSearch costDown = new ValueConfigSearch("[Equipment_Reverse_Down]Cost");
    private ResourceData costResourceDown;

    private List<UIInventorySlot> pools = new List<UIInventorySlot>();
    private EquipableItem currentItem;
    private EquipmentSave currentSaveItem;
    private EquipmentSave resultSaveItem;
    private InventorySave inventorySave;
    private BlackSmithUpgradeTable blackSmithDb;
    private ResourceUpgradeEquipment resourceReverse;
    private ResourcesSave resourceSave;

    public override void PostInit()
    {
        inventorySave = DataManager.Save.Inventory;
        resourceSave = DataManager.Save.Resources;
        blackSmithDb = DataManager.Base.BlackSmithUpgrade;
        costResourceDown = new LootParams(costDown.StringValue).Data as ResourceData;
        costPanel.Set(costResourceDown);
    }
    public override void Close()
    {
        foreach (var go in pools)
        {
            go.Clear();
            PoolManager.Instance.Despawn(go.gameObject);
        }
        pools.Clear();
        //equipmentSlot.Clear();
        equipmentSlotResult.Clear(); 
        base.Close();
    }
    public void Show(EquipableItem item)
    {
        base.Show();
        this.currentItem = item;
        currentSaveItem = item.EquipmentSave;
        SetInfor();
    }

    public void ReverseOnClicked()
    {
        var resource = DataManager.Save.Resources;
        if (resource.HasResource(costResourceDown))
        {
            resource.DecreaseResource(costResourceDown);
            currentSaveItem.Revert();
            GameSceneManager.Instance.EquipmentFactory.ReverseEquipment(currentItem);

            var loot = new List<LootParams>();

            var cost = resourceReverse.Currency;
            loot.AddRange(cost.GetAllData());

            var fragment = resourceReverse.Materials;
            foreach (var f in fragment)
            {
                loot.AddRange(f.GetAllData());
            }
            Messenger.Broadcast(EventKey.ReverseEquipment, currentSaveItem);

            onClosed += async () =>
            {
                await PanelManager.ShowRewards(loot);
            };

            Close();
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, costResourceDown.Resource.GetLocalize())).Forget();
            MenuGameScene.Instance.EnQueue(EFlashSale.Gem);
        }
    }

    private async void SetInfor()
    {
        // spawn icon
        equipmentLevelTxt.text = $"Lv. {currentItem.ItemLevel}/{currentItem.EquipmentRarity.GetMaxLevel()}";
        equipmentSlot.Clear();

        // Get icon equipment
        var generalItemPrefab = await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIGeneralEquipmentItem);
        var general = PoolManager.Instance.Spawn(generalItemPrefab).GetComponent<UIGeneralEquipmentIcon>();
        general.Init(currentItem);
        equipmentSlot.SetItem(general);

        // Set main equipment result
        resultSaveItem = new EquipmentSave
        {
            Id = currentItem.Id,
            Level = 0,
            Rarity = currentItem.EquipmentRarity,
        };

        var result = resultSaveItem.CreateInstanceEquipableItem();
        var resultIns = PoolManager.Instance.Spawn(generalItemPrefab).GetComponent<UIGeneralEquipmentIcon>();
        resultIns.Init(result);
        equipmentSlotResult.SetItem(resultIns);

        // Get cost from level 1 -> current level
        int levelCurrent = currentItem.ItemLevel;
        resourceReverse = blackSmithDb.GetResourceToLevel(currentItem.EquipmentType, levelCurrent);

        var coin = resourceReverse.Currency;
        var materials = resourceReverse.Materials;

        // coin
        if (coin.Value != 0)
        {
            // spawn slot
            var inventory = await UIHelper.GetUILootItem(string.Format(AddressableName.UILootItemPath, "Resource"), coin, holderResult);
            inventory.gameObject.SetActive(true);
            pools.Add(inventory);
        }

        if (materials.Count > 0)
        {
            // spawn slot
            foreach (var material in materials)
            {
                var inventory = await UIHelper.GetUILootItem(string.Format(AddressableName.UILootItemPath, "Fragment"), material, holderResult);
                inventory.gameObject.SetActive(true);
                pools.Add(inventory);
            }
        }
    }
}
