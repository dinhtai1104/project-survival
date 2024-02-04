using UnityEngine;
using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

// Recycle Item
public class UIInventoryRecycleRow : UIBehaviour, IDynamicScrollViewItem
{
    protected int m_Index = -1;
    [SerializeField] protected List<RectTransform> slotsInventory = new List<RectTransform>();
    [SerializeField] protected UIInventoryBag parentView;
    protected List<UIInventorySlot> slots = new List<UIInventorySlot>();
    protected List<EquipableItem> equipableItems = new List<EquipableItem>();
    private CancellationTokenSource ctks = new CancellationTokenSource();

    protected override void OnEnable()
    {
        base.OnEnable();
        Messenger.AddListener<EquipmentSave>(EventKey.ReverseEquipment, OnUpdate);
        //Messenger.AddListener<EResource>(EventKey.UpdateResource, OnUpdateResource);
    }

    private void OnUpdateResource(EResource type)
    {
        UpdateEquipment();
    }

    private void OnUpdate(EquipmentSave eq)
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                var uieq = slot.Get<UIGeneralEquipmentIcon>().Item;
                if (uieq.EquipmentSave.IdSave == eq.IdSave)
                {
                    slot.Get<UIGeneralEquipmentIcon>().OnUpdate();
                }
            //    slot.SetNoti(CheckCanEnhance(uieq));
            }
        }
    }

    protected override void OnDisable()
    {
        Messenger.RemoveListener<EquipmentSave>(EventKey.ReverseEquipment, OnUpdate);
        //Messenger.RemoveListener<EResource>(EventKey.UpdateResource, OnUpdateResource);
        base.OnDisable();
    }

    public int getIndex()
    {
        return m_Index;
    }

    public void onUpdateItem(int index)
    {
        if (index > parentView.ItemsIndexRow.Count) return;
        m_Index = index;
        UpdateEquipment();
    }

    protected virtual void UpdateEquipment()
    {
        if (m_Index == -1) return;
        ctks.Cancel();
        ctks = new CancellationTokenSource();
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
                        (t.Item as UIGeneralEquipmentIcon).Init(equipData);
                        t.gameObject.SetActive(true);
                        t.OnItemClick += parentView.OnSlotClick;
                      //  t.SetNoti(CheckCanEnhance(data));
                    }).AttachExternalCancellation(ctks.Token);
            }
        }
    }

    private bool CheckCanEnhance(EquipmentData data)
    {
        var res = GameSceneManager.Instance.EquipmentFactory.CheckEnhance(data);

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
    private bool CheckCanEnhance(EquipableItem data)
    {
        var res = GameSceneManager.Instance.EquipmentFactory.CheckEnhance(data);

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
}