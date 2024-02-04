using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Slot in equiped HeroItem
/// </summary>
public class UIInventoryEquipmentSlot : UIBaseSlotItem
{
    [SerializeField] private EEquipment equipmentType;

    [SerializeField] private UIInventorySlot slot;
    public EEquipment Type => equipmentType;

    public UIInventoryEquipmentItem SlotItem => slotItem;

    private UIInventoryEquipmentItem slotItem = null;
    private void OnEnable()
    {
        SetIcon(ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentType, $"{equipmentType}"));
        slot.OnItemClick += OnItemClick;
    }
    private void OnDisable()
    {
        slot.OnItemClick -= OnItemClick;
    }

    private async void OnItemClick(UIInventorySlot slot, UIGeneralBaseIcon item)
    {
        var equipment = item as UIInventoryEquipmentItem;
        if (equipment == null)
        {
            return;
        }
        Debug.Log("Show Item: " + equipment.Item);
        var uiInfo = await UI.PanelManager.CreateAsync<UIEquipmentInforPanel>(AddressableName.UIEquipmentInforPanel);
        uiInfo.Show(equipment.Item);
    }

    public async void Equip(EquipableItem item)
    {
        slot.Clear();
        var inventoryPrefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIInventoryEquipmentItem)).GetComponent<UIInventoryEquipmentItem>();
        if (inventoryPrefab == null) return;
        slotItem = PoolManager.Instance.Spawn(inventoryPrefab, transform);

        slotItem.Init(item);
        slot.SetItem(slotItem);
        slot.SetNoti(CheckNoti(item));

    }

    private bool CheckNoti(EquipableItem item)
    {
        var res = GameSceneManager.Instance.EquipmentFactory.CheckEnhance(item);

        switch (res)
        {
            case EEhanceResult.Success:
                return true;
            case EEhanceResult.MaxLevel:
                return false;
            case EEhanceResult.NotEnoughCurrency:
                return false;
            case EEhanceResult.NotEnoughFragment:
                return false;
        }
        return false;
    }
    public void UnEquip()
    {
        if (slotItem != null)
        {   
            slot.Clear();
        }
        slot.SetNoti(false);
        slotItem = null;
    }
}