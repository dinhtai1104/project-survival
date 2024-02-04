using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EquipmentSave
{
    public int IdSave;
    public string Id;
    public int Level;
    public ERarity Rarity;
    public bool IsEquiped = false;

    [NonSerialized, ShowInInspector] public bool IsSourceCrafting;
    [NonSerialized, ShowInInspector] public bool IsMaterialCrafting;
    public EquipmentEntity GetEquipment()
    {
        return DataManager.Base.Equipment.Dictionary[Id];
    }
    public void Save()
    {
        DataManager.Save.Inventory.Save();
    }

    public void LevelUp(bool save = false)
    {
        Level++;
        if (save)
        {
            Save();
        }
    }

    public void SetRarity(ERarity newRarity, bool save = false)
    {
        Rarity = newRarity;
        if (save)
        {
            Save();
        }
    }

    public void Equip(bool save = false)
    {
        IsEquiped = true;
        if (save)
        {
            Save();
        }
    }

    public void UnEquip(bool save = false)
    {
        IsEquiped = false;
        if (save)
        {
            Save();
        }
    }

    public EquipableItem CreateEquipableItem()
    {
        var equipable = DataManager.Base.Equipment.GetItem(this.Id);
        equipable.ApplySaveData(this);

        equipable.EnhanceEvent += Save;
        equipable.UpRarityEvent += Save;

        return equipable;
    }

    public EquipableItem CreateInstanceEquipableItem()
    {
        var equipable = DataManager.Base.Equipment.GetItem(this.Id);
        equipable.ApplySaveData(this);

        return equipable;
    }

    public EquipableItem CreateInstanceWithStats()
    {
        var equipable = DataManager.Base.Equipment.GetItem(this.Id);
        equipable.ApplySaveData(this);

        return equipable;
    }
 
    public EquipmentSave Clone()
    {
        return new EquipmentSave { Id = this.Id, Level = Level, Rarity = Rarity, IdSave = IdSave };
    }

    public void Revert()
    {
        Level = 0;
        Save();
    }

    internal void RarityUp()
    {
        Rarity++;
        Save();
    }

    public bool IsReadyMerge(EquipmentSave chooseSave)
    {
        if (chooseSave.Id == this.Id && chooseSave.Rarity == Rarity) return true;
        return false;
    }
}