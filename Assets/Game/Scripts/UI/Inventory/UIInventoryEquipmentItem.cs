using Spine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryEquipmentItem : UIGeneralEquipmentIcon
{
    [SerializeField] private GameObject upgradableObj;
    [SerializeField] private GameObject isEquipedGO;

    private void OnEnable()
    {
        Messenger.AddListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
        Messenger.AddListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverse);
        Messenger.AddListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquip);
        Messenger.AddListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipment);
    }

    private void OnUnEquipment(EEquipment arg1)
    {
        if (item != null && arg1 == item.EquipmentType)
        {
            SetInformation();
        }
    }

    private void OnEquip(EEquipment arg1, EquipableItem arg2)
    {
        if (item != null && equimentData != null && arg2.EquipmentSave == item.EquipmentSave && arg1 == item.EquipmentType)
        {
            SetInformation();
        }
    }

    private void OnReverse(EquipmentSave arg1)
    {
        if (equimentData != null)
        {
            SetInformation();
        }
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<EquipableItem>(EventKey.EnhanceSuccess, OnEnhanceSuccess);
        Messenger.RemoveListener<EquipmentSave>(EventKey.ReverseEquipment, OnReverse);
        Messenger.RemoveListener<EEquipment, EquipableItem>(EventKey.EquipItem, OnEquip);
        Messenger.RemoveListener<EEquipment>(EventKey.UnEquipItem, OnUnEquipment);
    }

    private void OnEnhanceSuccess(EquipableItem item)
    {
        if (item == this.Item)
        {
            SetInformation();
        }
    }

    public override void SetInformation()
    {
        if (item != null)
        {
            base.SetInformation();
        }
    }

    public override void Clear()
    {
        base.Clear();
        if (Item != null)
        {
            item = null;
        }
    }
}