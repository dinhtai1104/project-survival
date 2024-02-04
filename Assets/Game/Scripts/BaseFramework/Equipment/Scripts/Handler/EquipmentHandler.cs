using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentHandler
{
    private IStatGroup _stat;
    [ShowInInspector]
    protected Dictionary<EEquipment, EquipableItem> _equipmentSlots;
    [SerializeField]
    private List<EquipableItem> _equipments;

    public EquipmentHandler(IStatGroup stat)
    {
        this._stat = stat;
        _equipmentSlots = new Dictionary<EEquipment, EquipableItem>();
        foreach (var type in (EEquipment[])Enum.GetValues(typeof(EEquipment)))
        {
            _equipmentSlots.Add(type, null);
        }
        _equipments = new List<EquipableItem>();
    }

    public EquipmentHandler Clone(IStatGroup stat)
    {
        var eq = new EquipmentHandler(stat);
        foreach (var e in _equipments)
        {
            eq.EquipClone(e.CloneShallow());
        }
        return eq;
    }

    public void EquipClone(EquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Equip null Item");
            return;
        }

        if (_equipmentSlots.ContainsKey(item.EquipmentType))
        {
            var currentItem = _equipmentSlots[item.EquipmentType];

            if (currentItem != null)
            {
                Unequip(currentItem.EquipmentType);
            }
        }

        _equipments.Add(item);
        _equipmentSlots[item.EquipmentType] = item;

        item.OnEquip(_stat);
    }

    public void Equip(EquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Equip null Item");
            return;
        }

        if (_equipmentSlots.ContainsKey(item.EquipmentType))
        {
            var currentItem = _equipmentSlots[item.EquipmentType];

            if (currentItem != null)
            {
                Unequip(currentItem.EquipmentType);
            }
        }
        else
        {
            _equipmentSlots.Add(item.EquipmentType, null);
        }

        item.IsEquipped = true;
        _equipments.Add(item);
        _equipmentSlots[item.EquipmentType] = item;
        item.EquipmentSave.Equip(true);

        item.OnEquip(_stat);
        Messenger.Broadcast(EventKey.EquipItem, item.EquipmentType, item);
    }
    public void Unequip(EEquipment slot)
    {
        if (!_equipmentSlots.ContainsKey(slot))
        {
            _equipmentSlots.Add(slot, null);
        }

        var equipmentSlot = _equipmentSlots[slot];

        if (equipmentSlot == null) return;

        EquipableItem item = equipmentSlot;
        item.IsEquipped = false;
        _equipments.Remove(item);
        _equipmentSlots[slot] = null;
        item.EquipmentSave.UnEquip(true);

        item.OnUnequip();
        Messenger.Broadcast(EventKey.UnEquipItem, item.EquipmentType);
    }
    public EquipableItem GetEquipment(EEquipment equipmentType)
    {
        try 
        {
            return _equipmentSlots[equipmentType];
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public bool HasEquipmentType(EEquipment e)
    {
        if (_equipmentSlots.ContainsKey(e))
        {
            return _equipmentSlots[e] != null;
        }
        return false;
    }
    public EquipableItem GetEquipment(string id)
    {
        return _equipments.Find(t => t.Id == id);
    }

    public async void EquipPassive(ActorBase player,params EEquipment [] includeOnlyTypes)
    {
        foreach (var slot in _equipmentSlots)
        {
            var slotType = slot.Key;
            var item = slot.Value;
            if (item == null) continue;
            if(includeOnlyTypes != null  && includeOnlyTypes.Length>0 && !includeOnlyTypes.Contains(slotType))
            {
                continue;
            }

            var entity = item.EquipmentEntity;
            var passive = entity.Passive;
            if (string.IsNullOrEmpty(passive)) continue;

            var obj = await ResourcesLoader.Instance.GetGOAsync("Equipment/" + passive + ".prefab");
            obj.transform.SetParent(player.GetMidTransform());
            obj.transform.localPosition = Vector3.zero;
            var passiveIns = obj.GetComponent<BaseEquipmentPassive>();
            passiveIns.SetEquipment(item);

            player.PassiveEngine.ApplyPassive(passiveIns);
        }
    }
}
