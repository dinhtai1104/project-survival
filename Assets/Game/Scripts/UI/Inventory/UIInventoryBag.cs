using Mosframe;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIInventoryBag : MonoBehaviour
{
    public int ItemPerRow = 5;
    protected UIInventorySlot inventorySlotPrefab;
    [SerializeField] protected RectTransform holder;
    public UIGeneralEquipmentIcon equipmentItemPrefab;
    protected List<UIInventorySlot> inventorySlots = new List<UIInventorySlot>();
    protected List<EquipableItem> items = new List<EquipableItem>();

    private List<List<int>> itemsIndexRow = new List<List<int>>();
    public List<List<int>> ItemsIndexRow => itemsIndexRow;
    protected DynamicVScrollView scrollView;
    private void Awake()
    {
        scrollView = GetComponentInChildren<DynamicVScrollView>();
    }

    protected virtual void OnEnable()
    {
        Messenger.AddListener(EventKey.UpdateBag, OnUpdateBag);
    }
    protected virtual void OnDisable()
    {
        Messenger.RemoveListener(EventKey.UpdateBag, OnUpdateBag);
    }

    private void OnUpdateBag()
    {
        Show();
    }

    public virtual void Show()
    {
        PrepareIndex();
    }

    public EquipableItem GetItemInIndex(int index)
    {
        return items[index];
    }

    private void PrepareIndex()
    {
        ItemsIndexRow.Clear();
        int rowC = items.Count / ItemPerRow;
        int indexItem = 0;
        for (int i = 0; i < rowC; i++)
        {
            var data = new List<int>();
            for (int j = 0; j < ItemPerRow; j++)
            {
                data.Add(indexItem++);
            }
            itemsIndexRow.Add(data);
        }

        if (indexItem < items.Count)
        {
            var data = new List<int>();
            for (int j = indexItem; j < items.Count; j++)
            {
                data.Add(indexItem++);
            }
            itemsIndexRow.Add(data);
        }
    }

    public void Clear()
    {
        // Despawn ui slot
        foreach (var slot in inventorySlots)
        {
            slot.Clear();
            slot.OnItemClick -= OnSlotClick;
            PoolManager.Instance.Despawn(slot.gameObject);
        }
        inventorySlots.Clear();
    }

    public abstract void OnSlotClick(UIInventorySlot slot, UIGeneralBaseIcon item);
}