using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryAllEquipementMergeRarityView : UIInventoryBag
{
    [SerializeField] private UIMergeRarityView mergeRarityView;
    protected override void OnEnable()
    {
        base.OnEnable();
        Messenger.AddListener<int>(EventKey.RarityMerge_UnPickItem, OnUnPickItem);
        Messenger.AddListener(EventKey.RarityMerge_UnPickMainItem, OnUnPickMainItem);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Messenger.RemoveListener<int>(EventKey.RarityMerge_UnPickItem, OnUnPickItem);
        Messenger.RemoveListener(EventKey.RarityMerge_UnPickMainItem, OnUnPickMainItem);

        DataManager.Save.Inventory.RemoveAllFromMergeRarity();
    }


    private void OnUnPickMainItem()
    {
        foreach (var slot in inventorySlots)
        {
            slot.Get<UIGeneralEquipmentIcon>().SetLocked(false);
        }
    }


    private void OnUnPickItem(int equipmentSaveId)
    {
        foreach (var slot in inventorySlots)
        {
            var item = slot.Get<UIGeneralEquipmentIcon>();
            if (item.Item.IdSave == equipmentSaveId)
            {
                item.SetLocked(false);
                return;
            }
        }
    }
    public async override void Show()
    {
        items = DataManager.Save.Inventory.AllEquipableItem;
        items = items.OrderByDescending(x => CheckDotElvolveItemHandle(x.EquipmentSave))
            .ThenByDescending(x => x.EquipmentRarity).ThenBy(x => x.Id).ToList();

        base.Show();
        scrollView.init(ItemsIndexRow.Count);
        scrollView.refresh();
        return;

        items = DataManager.Save.Inventory.AllEquipableItem;
        base.Show();
        Clear();
        inventorySlotPrefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIInventorySlot)).GetComponent<UIInventorySlot>();
        foreach (var data in items)
        {
            var ins = PoolManager.Instance.Spawn(inventorySlotPrefab, holder);
            var equipIns = PoolManager.Instance.Spawn(equipmentItemPrefab);

            equipIns.Init(data);
            ins.SetItem(equipIns);
            ins.OnItemClick += OnSlotClick;
            inventorySlots.Add(ins);
        }
    }
    public override void OnSlotClick(UIInventorySlot slot, UIGeneralBaseIcon item)
    {
        var equipment = item as UIGeneralEquipmentIcon;
        if (equipment == null) { return; }
        if (equipment.IsPicked) { return; }
        if (equipment.IsLocked) { return; }
        if (equipment.Item.EquipmentRarity >= ERarity.Ultimate) return;
        if (mergeRarityView.IsFullItem()) return;
        equipment.SetLocked(true);
        Debug.Log($"Pick item {equipment.Item} to merge");
        Messenger.Broadcast(EventKey.RarityMerge_PickItem, equipment.Item);
    }


    private bool CheckDotElvolveItemHandle(EquipmentSave save)
    {
        var count = 0;
        var equipmentSave = DataManager.Save.Inventory;
        foreach (var equipment in equipmentSave.Saves)
        {
            if (save == equipment) continue;
            if (equipment.IsReadyMerge(save))
            {
                count++;
                if (count < 2) continue;
                return true;
            }
        }
        return false;

    }
}