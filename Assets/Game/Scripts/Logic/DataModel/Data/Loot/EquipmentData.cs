using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EquipmentData : ILootData
{
    public int value = 1;
    public double ValueLoot => value;
    public EquipmentEntity EquipmentEntity;
    public ERarity Rarity;
    public int Id;
    public string IdString;
    public int Level;
    public EEquipment Type;
    public bool IsRandom;
    public bool IsRandomType;

    public bool CanMergeData => false;
    public EquipableItem GetItem()
    {
        EquipmentEntity = DataManager.Base.Equipment.Get(IdString);
        var item = new EquipableItem(EquipmentEntity);
        var itemSave = new EquipmentSave { Id = EquipmentEntity.Id, Level = Level, Rarity = Rarity };
        item.EquipmentSave = itemSave;
        return item;
    }

    public List<LootParams> GetAllData()
    {
        EquipmentData data = CloneData() as EquipmentData;
        if (data.IsRandomType)
        {
            data.Type = ((EEquipment[])Enum.GetValues(typeof(EEquipment))).ToArray().Random();
        }
        if (data.Id == -1)
        {
            var db = DataManager.Base.Equipment.Dictionary.Values.ToList().FindAll(t => t.Type == data.Type).ToList();
            var EquipmentEntity = db.Random();
            data.Id = EquipmentEntity.IdNum;
            data.IdString = EquipmentEntity.Id;
            data.Type = EquipmentEntity.Type;
            data.Rarity = Rarity;
        }
        return new List<LootParams>() { new LootParams(ELootType.Equipment, data) };
    }
    public EquipmentData()
    {

    }
    public EquipmentData(List<string> param) : base()
    {
        Enum.TryParse(param[0], out Rarity);
        // Equipment;Rare;10
        if (int.TryParse(param[1], out Id))
        {
            Level = 0;
            if (param.Count > 2)
            {
                int.TryParse(param[2], out Level);
            }

            if (Id == -1)
            {
                // Random Type Equipment And Random Equipment
                //var type = ((EEquipment[])Enum.GetValues(typeof(EEquipment))).Random();
                //Type = type;
                IdString = "-1";
                IsRandom = true;
                IsRandomType = true;
                return;
            }


            EquipmentEntity = DataManager.Base.Equipment.GetEntity(Id).Clone();
            Type = EquipmentEntity.Type;
        }
        // Random Equipment
        // Equipment;Rare;Hat;10
        else if (Enum.TryParse(param[1], out Type))
        {
            if (param.Count > 2)
            {
                int.TryParse(param[2], out Id);
                if (Id == -1)
                {
                    IdString = "-1";
                    IsRandom = true;
                }
                else
                {
                    EquipmentEntity = DataManager.Base.Equipment.GetEntity(Id).Clone();
                    IdString = EquipmentEntity.Id;
                }
                Level = 0;
                if (param.Count > 3)
                {
                    int.TryParse(param[3], out Level);
                }
            }
        }
    }

    public EquipmentData(string idStr, ERarity rarity, int level = 0)
    {
        this.Rarity = rarity;
        EquipmentEntity = DataManager.Base.Equipment.GetEntity(idStr).Clone();
        Id = EquipmentEntity.IdNum;
        IdString = EquipmentEntity.Id;
        Type = EquipmentEntity.Type;
        Level = level;
    }

    public bool Add(ILootData data)
    {
        var equi = data as EquipmentData;
        if (equi == null) return false;
        if (equi.Type == Type && equi.Level == Level && equi.Rarity == Rarity && equi.IsRandom == IsRandom 
            && equi.Id == Id && equi.IsRandomType == IsRandomType)
        {
            value += (int)data.ValueLoot;
            return true;
        }
        return false;
    }
    public override string ToString()
    {
        return $"{this.EquipmentEntity.Id} {Rarity}";
    }
    public void Loot()
    {
        DataManager.Save.Inventory.Add(new EquipmentSave { Id = IdString, Level = Level, Rarity = Rarity });
    }


    public ILootData CloneData()
    {
        return new EquipmentData { Id = Id, Level = Level, Rarity = Rarity, IsRandom = true, IdString = IdString, Type = Type, IsRandomType = IsRandomType };
    }

    public string GetParams()
    {
        return $"Equipment;{Rarity};{Id};{Level}";
    }
    public EquipmentSave CreateSave()
    {
        return new EquipmentSave { Id = IdString, Level = Level, Rarity = Rarity };
    }
}