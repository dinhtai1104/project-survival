using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class UIInventoryMergeRow : UIInventoryRecycleRow
{
    private EquipmentSave chooseSave = null;
    private readonly List<int> craftingMaterials = new List<int>();
    public UIInventoryAllEquipementMergeRarityView Bag => base.parentView as UIInventoryAllEquipementMergeRarityView;
    protected override void Awake()
    {
        base.Awake();
        Messenger.AddListener<int>(EventKey.RarityMerge_PickMainItem, OnPickMainItem);
        Messenger.AddListener<int>(EventKey.RarityMerge_UnPickItem, OnUnPickItem);
        Messenger.AddListener(EventKey.RarityMerge_UnPickMainItem, OnUnPickMainItem);
        Messenger.AddListener(EventKey.RarityMerge_UpdateView, OnUpdateView);
        Messenger.AddListener<EquipableItem>(EventKey.RarityMerge_PickItem, OnPickItem);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Messenger.RemoveListener<int>(EventKey.RarityMerge_PickMainItem, OnPickMainItem);
        Messenger.RemoveListener(EventKey.RarityMerge_UnPickMainItem, OnUnPickMainItem);
        Messenger.RemoveListener<int>(EventKey.RarityMerge_UnPickItem, OnUnPickItem);
        Messenger.RemoveListener(EventKey.RarityMerge_UpdateView, OnUpdateView);
        Messenger.RemoveListener<EquipableItem>(EventKey.RarityMerge_PickItem, OnPickItem);
    }

    private void OnUnPickItem(int idSave)
    {
        var item = DataManager.Save.Inventory.FindSave(idSave);
        foreach (var slot in slots)
        {
            if (slot.Item != null)
            {
                var icon = slot.Get<UIGeneralEquipmentIcon>();
                var save = icon.Item.EquipmentSave;
                if (save.IdSave == item.IdSave)
                {
                    save.IsMaterialCrafting = false;
                    UpdateEquipment();
                    return;
                }
                //if (item.)
            }
        }
    }

    private void OnPickItem(EquipableItem item)
    {
        var saveChoose = item.EquipmentSave;
        foreach (var slot in slots)
        {
            if (slot.Item != null)
            {
                var icon = slot.Get<UIGeneralEquipmentIcon>();
                if (icon.IsLocked) { continue; }
                var save = icon.Item.EquipmentSave;
                if (item.IdSave == saveChoose.IdSave) continue;
                //if (item.)
            }
        }
    }

    private void OnUpdateView()
    {
        UpdateEquipment();
    }

    private void OnPickMainItem(int idSave)
    {
        var equipmentSave = DataManager.Save.Inventory;
        chooseSave = DataManager.Save.Inventory.FindSave(idSave);
        if (chooseSave == null) return;
        chooseSave.IsSourceCrafting = true;
        foreach (var equipment in equipmentSave.Saves)
        {
            if (equipment.IsReadyMerge(chooseSave))
            {
                craftingMaterials.Add(equipment.IdSave);
            }
        }
    }

    private void OnUnPickMainItem()
    {
        craftingMaterials.Clear();
        chooseSave = null;
        foreach (var slot in slots)
        {
            if (slot.Item != null)
            {
                slot.Get<UIGeneralEquipmentIcon>().SetLocked(false);
                slot.Get<UIGeneralEquipmentIcon>().SetPicked(false);
            }
        }
    }



    protected override void UpdateEquipment()
    {
        if (m_Index == -1) return;
        foreach (var slot in slots)
        {
            slot.OnItemClick -= parentView.OnSlotClick;
            slot.Clear();
            PoolManager.Instance.Despawn(slot.gameObject);
        }
        slots.Clear();
        equipableItems.Clear();

        for (int i = 0; i < slotsInventory.Count; i++)
        {
            if (parentView.ItemsIndexRow.Count > m_Index)
            {
                if (parentView.ItemsIndexRow[m_Index].Count > i)
                {
                    var index = parentView.ItemsIndexRow[m_Index][i];
                    var equipData = parentView.GetItemInIndex(index);
                    if (equipData == null) continue;
                    equipableItems.Add(equipData);
                    var data = new EquipmentData(equipData.Id, equipData.EquipmentRarity, equipData.ItemLevel);
                    var parent = slotsInventory[i];
                    UIHelper.GetUILootItem(AddressableName.UIGeneralEquipmentItem, data, parent)
                        .ContinueWith(t =>
                        {
                            slots.Add(t);
                            var itemEqIcon = (t.Item as UIGeneralEquipmentIcon);
                            itemEqIcon.Init(equipData);
                            t.gameObject.SetActive(true);
                            t.OnItemClick += parentView.OnSlotClick;
                            var save = equipData.EquipmentSave;
                            CheckDotElvolveItemHandle(itemEqIcon);

                            if (chooseSave != null)
                            {
                                if (save.IsSourceCrafting)
                                {
                                    itemEqIcon.SetLocked(true);
                                    itemEqIcon.SetPicked(true);
                                }
                                else
                                {
                                    if (save.IsMaterialCrafting)
                                    {
                                        itemEqIcon.SetLocked(true);
                                        itemEqIcon.SetPicked(true);
                                    }
                                    else
                                    {
                                        if (craftingMaterials.Contains(save.IdSave))
                                        {
                                            itemEqIcon.SetLocked(false);
                                        }
                                        else
                                        {
                                            itemEqIcon.SetLocked(true);
                                        }
                                        itemEqIcon.SetPicked(false);
                                    }
                                }
                            }

                        }).Forget();
                }
            }
        }
    }

    private void CheckDotElvolveItemHandle(UIGeneralEquipmentIcon icon)
    {
        var canEvolve = false;
        var count = 0;
        var equipmentSave = DataManager.Save.Inventory;
        foreach (var equipment in equipmentSave.Saves)
        {
            if (icon.Item.EquipmentSave == equipment) continue;
            if (equipment.IsReadyMerge(icon.Item.EquipmentSave))
            {
                count++;
                if (count < 2) continue;
                canEvolve = true;
                break;
            }
        }

        icon.ParentSlot.SetNoti(canEvolve);
    }
}