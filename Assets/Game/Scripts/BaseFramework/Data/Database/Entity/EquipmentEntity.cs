using System;
using System.Collections.Generic;

[System.Serializable]
public class EquipmentEntity
{
    public int IdNum;
    public string Id;
    public EEquipment Type;
    public StatKey StatKey;
    public float StatBase;
    public EStatMod StatMod;
    public float StatGrown;
    public EResource FragmentType;

    //public List<PassiveData> Passives;
    public string DescriptionKey;

    public Dictionary<ERarity, BaseAffix> Affixes = new Dictionary<ERarity, BaseAffix>();
    public string Passive;

    private DB_Equipment entity;
    public EquipmentEntity(DB_Equipment e)
    {
        this.entity = e;
        IdNum = e.Get<int>("IdNum");
        Id = e.Get<string>("Id");
        Enum.TryParse(e.Get<string>("Type"), out Type);
        Enum.TryParse(e.Get<string>("FragmentType"), out FragmentType);

        // Stat
        Enum.TryParse(e.Get<string>("StatName"), out StatKey);
        Enum.TryParse(e.Get<string>("StatMod"), out StatMod);
        StatBase = e.Get<float>("StatBase");
        StatGrown = e.Get<float>("StatGrown");

        DescriptionKey = $"{"Base"}_{StatKey}_{StatMod}";

        AddAffix(ERarity.UnCommon, ERarity.UnCommon.ToString(), e);
        AddAffix(ERarity.Common, ERarity.Common.ToString(), e);
        AddAffix(ERarity.Rare, ERarity.Rare.ToString(), e);
        AddAffix(ERarity.Epic, ERarity.Epic.ToString(), e);
        AddAffix(ERarity.Legendary, ERarity.Legendary.ToString(), e);
        AddAffix(ERarity.Ultimate, ERarity.Ultimate.ToString(), e);

        Passive = e.Get<string>("Passive");
    }

    private void AddAffix(ERarity rarity, string keyTable, DB_Equipment entity)
    {
        var data = entity.Get<string>(keyTable);
        if (string.IsNullOrEmpty(data)) return;
        var dataType = entity.Get<string>($"{keyTable}Type");
        Enum.TryParse(dataType, out ETypeAffix AffixType);

        BaseAffix affix;
        switch (AffixType)
        {
            case ETypeAffix.Base:
                var attribute = new AttributeStatModifier(data);
                affix = new StatAffix(attribute);
                break;
            case ETypeAffix.Passive :
                affix = new PassiveAffix(data);
                break;
            default:
                affix = NullAffix.Null;
                break;
        }
        Affixes.Add(rarity, affix);
    }

    public EquipmentEntity Clone()
    {
        return new EquipmentEntity(entity);
    }
}
