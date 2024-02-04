using System;
using System.Collections.Generic;

[System.Serializable]
public class BlackSmithUpgradeTable : DataTable<EEquipment, Dictionary<int, BlackSmithUpgradeEntity>>
{
    public override void GetDatabase()
    {
        DB_BlackSmithUpgrade.ForEachEntity(e => Get(e));
    }

    private void Get(DB_BlackSmithUpgrade e)
    {
        var entity = new BlackSmithUpgradeEntity(e);

        if (!Dictionary.ContainsKey(entity.EquipmentType))
        {
            Dictionary.Add(entity.EquipmentType, new Dictionary<int, BlackSmithUpgradeEntity>());
        }
        if (!Dictionary[entity.EquipmentType].ContainsKey(entity.Level))
        {
            Dictionary[entity.EquipmentType].Add(e.EnhanceLevel, entity);
        }
    }


    public ResourceUpgradeEquipment GetResourceToLevel(EEquipment type, int level)
    {
        var value = new ResourceUpgradeEquipment();
        var list = Dictionary[type];
        for (int i = 0; i <= level; i++)
        {
            if (!list.ContainsKey(i)) continue;
            var entity = list[i];

            value.AddCoin(entity.GoldCost);
            if (entity.MaterialCost == null) continue;
            value.AddMaterial(entity.MaterialCost);
        }
        return value;
    }
    public BlackSmithUpgradeEntity GetResourceUpgradeEntity(EEquipment type, int level)
    {
        if (Dictionary[type].ContainsKey(level))
        {
            var entity = Dictionary[type][level];
            return entity;
        }
        return null;
    }
}
[System.Serializable]
public class ResourceUpgradeEquipment
{
    public ResourceData Currency;
    public List<ResourceData> Materials;

    public ResourceUpgradeEquipment()
    {
        Currency = new ResourceData { Resource = EResource.Gold, Value = 0 };
        Materials = new List<ResourceData>();
    }
    public void AddCoin(ILootData coin)
    {
        Currency.Add(coin);
    }
    public void AddMaterial(ILootData material)
    {
        var frag = material as ResourceData;
        var lootType = frag.Resource;
        var data = frag;

        for (int i = 0; i < Materials.Count; i++)
        {
            var ma = Materials[i];
            if (ma.Resource == lootType)
            {
                ma.Add(data);
                return;
            }
        }
        Materials.Add(new ResourceData { Resource = frag.Resource, Value = frag.Value });
    }
}
