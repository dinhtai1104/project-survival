using Cysharp.Threading.Tasks;
using Mosframe;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryAllEquipmentView : UIInventoryBag
{
    [SerializeField] private GameObject emptyButton;
    [SerializeField] private GameObject mergeButton;
    [SerializeField] private GameObject quickEquipButton;
    [SerializeField] private NotifyCondition mergeCondition;
    [SerializeField] private StrongerEquipmentNotifyCondition strongerCondition;
    protected override void OnEnable()
    {
        base.OnEnable();
        Messenger.AddListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.AddListener(EventKey.QuickEquip, OnQuickEquip);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messenger.RemoveListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverseEquipment);
        Messenger.RemoveListener(EventKey.QuickEquip, OnQuickEquip);
    }

    private void OnQuickEquip()
    {
        quickEquipButton.SetActive(false);
    }

    private void OnReverseEquipment(EquipmentSave equipment)
    {
        foreach (var slot in inventorySlots)
        {
            var eq = slot.Get<UIGeneralEquipmentIcon>();
            if (eq == null) continue;
            var save = eq.Item.EquipmentSave;
            if (save.Id == equipment.Id && save.IdSave == equipment.IdSave) 
            {
                eq.OnUpdate();
            }
        }
    }

    public async override void Show()
    {
        items = DataManager.Save.Inventory.AllEquipableItem.FindAll(t => !t.IsEquipped);
        if (items == null)
        {
            items = new List<EquipableItem>();
        }
        base.Show();
        try
        {
            mergeButton.SetActive(mergeCondition.Validate());
            quickEquipButton.SetActive(strongerCondition.Validate());
            emptyButton.gameObject.SetActive((items.IsNullOrEmpty()));
        }
        catch (Exception ex)
        {
            return;
        }
        items = items.OrderByDescending(x => x.EquipmentRarity)
            .ThenBy(x => x.EquipmentType).ToList();
        if (scrollView != null)
        {
            scrollView.totalItemCount = ItemsIndexRow.Count;
            scrollView.refresh();
        }
        return;
    }

    public override async void OnSlotClick(UIInventorySlot slot, UIGeneralBaseIcon item)
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

}
