using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UIMergeRarityView : MonoBehaviour
{
    [SerializeField] private UIInventorySlot resultSlot;
    [SerializeField] private UIInventorySlot sourceSlot;

    [SerializeField] private UIInventorySlot[] sourceSlotGroup;

    [SerializeField] private UIMergeRarityInforNewItemView inforNewItem;
    [SerializeField] private GameObject mergeButton;
    [SerializeField] private UIParticle[] particles;
    [SerializeField] private SetColorParticles particleToSetColor;
    [SerializeField] private GameObject _fadingBackground;

    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color uncommonColor = Color.white;
    [SerializeField] private Color rareColor = Color.white;
    [SerializeField] private Color epicColor = Color.white;
    [SerializeField] private Color legendaryColor = Color.white;
    [SerializeField] private Color ultimateColor = Color.white;

    private EquipableItem resultItem;

    private EquipableItem sourceItem;
    private List<EquipableItem> sourcePartItem;

    private void OnEnable()
    {
        sourcePartItem = new List<EquipableItem>();
        for (int i = 0; i < sourceSlotGroup.Length; i++)
        {
            sourcePartItem.Add(null);
        }
        Messenger.AddListener<EquipableItem>(EventKey.RarityMerge_PickItem, OnAddItem);
        sourceSlot.OnItemClick += OnMainSourceRemove;
        foreach (var part in sourceSlotGroup)
        {
            part.OnItemClick += OnRemoveItem;
        }
        mergeButton.gameObject.SetActive(true);
        resultSlot.gameObject.SetActive(false);
    }

    private void OnMainSourceRemove(UIInventorySlot slot, UIGeneralBaseIcon item)
    {
        sourceItem.EquipmentSave.IsSourceCrafting = false;
        sourceItem.EquipmentSave.IsMaterialCrafting = false;
        foreach (var s in sourcePartItem)
        {
            if (s == null) continue;
            s.EquipmentSave.IsMaterialCrafting = false;
        }
        inforNewItem.Clear();
        resultSlot.gameObject.SetActive(false);

        Messenger.Broadcast(EventKey.RarityMerge_UnPickItem, sourceItem.IdSave);
        Messenger.Broadcast(EventKey.RarityMerge_UnPickMainItem);
        resultSlot.Clear();
        sourceSlot.Clear();
        foreach (var part in sourceSlotGroup)
        {
            part.Clear();
        }

        resultItem = null;
        sourceItem = null;

        for (int i = 0; i < sourcePartItem.Count; i++)
        {
            if (sourcePartItem[i] != null)
            {
                Messenger.Broadcast(EventKey.RarityMerge_UnPickItem, sourcePartItem[i].IdSave);

            }
            sourcePartItem[i] = null;
        }
    }

    private void OnRemoveItem(UIInventorySlot slot, UIGeneralBaseIcon item)
    {
        for (int i = 0;i < sourcePartItem.Count;i++)
        {
            if (sourcePartItem[i] == (item as UIGeneralEquipmentIcon).Item)
            {
                Messenger.Broadcast(EventKey.RarityMerge_UnPickItem, sourcePartItem[i].IdSave);
                sourcePartItem[i] = null;
                break;
            }
        }
        slot.Clear();
    }

    private void OnDisable()
    {
        inforNewItem.Clear();
        resultSlot.gameObject.SetActive(false);
        resultItem = null;
        sourceItem = null;
        foreach (var part in sourceSlotGroup)
        {
            part.Clear();
        }
        Messenger.RemoveListener<EquipableItem>(EventKey.RarityMerge_PickItem, OnAddItem);

        sourceSlot.OnItemClick -= OnMainSourceRemove;
        foreach (var part in sourceSlotGroup)
        {
            part.OnItemClick -= OnRemoveItem;
        }
    }

    private void OnAddItem(EquipableItem item)
    {
        if (sourceItem == null)
        {
            item.EquipmentSave.IsSourceCrafting = true;
            SetSourceSlot(item);
            Messenger.Broadcast(EventKey.RarityMerge_PickMainItem, item.IdSave);
        }
        else
        {
            item.EquipmentSave.IsMaterialCrafting = true;
            AddSourceSlot(item);
        }
        Messenger.Broadcast(EventKey.RarityMerge_UpdateView);
    }

    private async void SetResultSlot(EquipableItem item)
    {
        resultItem = item;
        var itemSlot = await CreateEquipmentIcon(resultItem);
        itemSlot.Init(item);
        resultSlot.SetItem(itemSlot);
    }
    public async void SetSourceSlot(EquipableItem item)
    {
        //if (item.EquipmentRarity >= ERarity.Legendary) return;
        sourceItem = item;
        resultSlot.gameObject.SetActive(true);

        var itemSlot = await CreateEquipmentIcon(item);
        itemSlot.Init(item);
        sourceSlot.SetItem(itemSlot);

        // Create result item
        var resultItem = item.Clone();

        foreach (var line in resultItem.LineAffixes)
        {
            if (line.Value is StatAffix)
            {
                (line.Value as StatAffix).Stats = null;
            }
        }

        var factory = GameSceneManager.Instance.EquipmentFactory;
        factory.ApplyLevelItem(resultItem, sourceItem.ItemLevel);
        factory.ApplyRarityItem(resultItem, sourceItem.EquipmentRarity + 1);

        SetResultSlot(resultItem);
        inforNewItem.Set(resultItem);
    }

    private async UniTask<UIGeneralEquipmentIcon> CreateEquipmentIcon(EquipableItem item)
    {
        var prefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIGeneralEquipmentItem)).GetComponent<UIGeneralEquipmentIcon>();
        var ins = PoolManager.Instance.Spawn(prefab, transform);
        return ins;
    }

    public async void AddSourceSlot(EquipableItem item)
    {
        for (int i = 0; i < sourcePartItem.Count; i++)
        {
            if (sourcePartItem[i] == null)
            {
                var itemSlot = await CreateEquipmentIcon(item);
                sourcePartItem[i] = item;

                itemSlot.Init(item);
                sourceSlotGroup[i].SetItem(itemSlot);
                break;
            }
        }
    }

    public async void MergeOnClicked()
    {
        if (IsFullItem() == false)
        {
            Debug.Log("MERGE::: NOT FILL ENOUGH ITEM");
            UI.PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/UpgradeEquip_Not fill equipment enough")).Forget();
            return;
        }
        _fadingBackground.SetActive(true);
        Debug.Log("MERGE::: MERGE ON CLICK: RESULT: " + resultItem?.ToString() ?? "Null");
        var inventorySave = DataManager.Save.Inventory;

        bool isEquip = false;

        if (sourceItem.IsEquipped)
        {
            isEquip = true;
            GameSceneManager.Instance.PlayerData.EquipmentHandler.Unequip(sourceItem.EquipmentType);
        }
        foreach (var partItem in sourcePartItem)
        {
            if (partItem.IsEquipped)
            {
                isEquip = true;
                GameSceneManager.Instance.PlayerData.EquipmentHandler.Unequip(sourceItem.EquipmentType);
            }
        }


        int newLevel = sourceItem.ItemLevel;
        for (int i = 0; i < sourcePartItem.Count; i++)
        {
            newLevel = Mathf.Max(newLevel, sourcePartItem[i].ItemLevel);
            inventorySave.RemoveItem(sourcePartItem[i].IdSave);
            sourcePartItem[i] = null;
        }
        inventorySave.RemoveItem(sourceItem.IdSave);
        sourceItem = null;
        var newEq = inventorySave.AddEquipment(resultItem.Id, resultItem.EquipmentRarity, newLevel);

        if (isEquip)
        {
            GameSceneManager.Instance.PlayerData.EquipmentHandler.Equip(newEq);
        }
        mergeButton.gameObject.SetActive(false);
        SetColorLabel();
        foreach (var part in particles)
        {
            part.Play();
        }

        var resultPanel = await PanelManager.CreateAsync<UIMergeRarityResultPanel>(AddressableName.UIMergeRarityResultPanel);

        await UniTask.Delay(TimeSpan.FromSeconds(1.8f));
        Messenger.Broadcast(EventKey.UpdateBag);
        Messenger.Broadcast(EventKey.RarityMerge_UnPickMainItem);
        inventorySave.Save();
        Clear();

        resultPanel.Show(newEq);
        mergeButton.gameObject.SetActive(true);
        _fadingBackground.SetActive(false);
    }

    private void SetColorLabel()
    {
        Color c = Color.white;
        switch (resultItem.EquipmentRarity)
        {
            case ERarity.Common:
                c = commonColor;
                break;
            case ERarity.UnCommon:
                c = uncommonColor;
                break;
            case ERarity.Rare:
                c = rareColor;
                break;
            case ERarity.Epic:
                c = epicColor;
                break;
            case ERarity.Legendary:
                c = legendaryColor;
                break;
            case ERarity.Ultimate:
                c = ultimateColor;
                break;
        }
        particleToSetColor.SetColor(c);
    }

    private void Clear()
    {
        foreach (var par in particles)
        {
            par.Clear();
        }

        inforNewItem.Clear();
        resultSlot.Clear();
        sourceSlot.Clear();
        foreach (var part in sourceSlotGroup)
        {
            part.Clear();
        }

        sourceItem = null;
        for (int i = 0; i < sourcePartItem.Count; i++)
        {
            sourcePartItem[i] = null;
        }
    }

    public bool IsFullItem()
    {
        if (sourceItem == null) return false;
        foreach (var source in sourcePartItem)
        {
            if (source == null) return false;
        }
        return true;
    }
}