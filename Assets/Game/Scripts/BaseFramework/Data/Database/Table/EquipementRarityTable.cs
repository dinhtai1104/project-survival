using System;
using System.Collections.Generic;

[System.Serializable]
public class EquipmentRarityTable : DataTable<ERarity, RarityEquipmentEntity>
{
    public override void GetDatabase()
    {
        DB_EquipmentRarity.ForEachEntity(e => Get(e));
    }

    public RarityEquipmentEntity GetLevel(ERarity oldRarity)
    {
        return Dictionary[oldRarity];
    }

    private void Get(DB_EquipmentRarity e)
    {
        var entity = new RarityEquipmentEntity(e);
        Dictionary.Add(entity.Rarity, entity);
    }
}
