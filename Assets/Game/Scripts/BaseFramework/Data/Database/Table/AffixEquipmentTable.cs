using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class AffixEquipmentTable : DataTable<string, AffixEquipmentEntity>
{
    [ShowInInspector]
    public Dictionary<string, AffixGroupFilterData> AffixGroupFilterEquipment = new Dictionary<string, AffixGroupFilterData>();
    public override void GetDatabase()
    {
        DB_AffixEquipment.ForEachEntity(e => Get(e));
    }

    public bool HasAffixEquipment(string id)
    {
        return AffixGroupFilterEquipment.ContainsKey(id);
    }

    private void Get(DB_AffixEquipment e)
    {
        var affix = new AffixEquipmentEntity(e);
        Dictionary.Add(affix.Id, affix);
        foreach (var eq in affix.Equipment)
        {
            if (!AffixGroupFilterEquipment.ContainsKey(eq))
            {
                AffixGroupFilterEquipment.Add(eq, new AffixGroupFilterData());
            }
            AffixGroupFilterEquipment[eq].AddAffix(affix.Rarity, affix);
        }
    }
}

[System.Serializable]
public class AffixGroupFilterData
{
    public int IdEquipment;
    public Dictionary<ERarity, List<AffixEquipmentEntity>> Affixes;
    public AffixGroupFilterData()
    {
        Affixes = new Dictionary<ERarity, List<AffixEquipmentEntity>>();
        var allEntity = (ERarity[])Enum.GetValues(typeof(ERarity));
        foreach (var e in allEntity)
        {
            Affixes.Add(e, new List<AffixEquipmentEntity>());
        }
    }
    public void AddAffix(ERarity rarity, AffixEquipmentEntity e)
    {
        Affixes[rarity].Add(e);
    }

    public List<AffixEquipmentEntity> GetFilter(ERarity rarity)
    {
        return Affixes[rarity];
    }

    ~AffixGroupFilterData()
    {
        Affixes.Clear();
        Affixes = null;
    }
}
