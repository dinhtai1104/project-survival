using System;
using System.Collections.Generic;

[System.Serializable]
public class SkillTreeLootData : ILootData
{
    public StatKey statKey;
    public StatModifier statModifier;
    public SkillTreeLootData() { }
    public SkillTreeLootData(StatKey statKey, StatModifier statModifier) : this()
    {
        this.statKey = statKey;
        this.statModifier = statModifier;
    }

    public double ValueLoot => 0;

    public bool CanMergeData => false;

    public bool Add(ILootData data)
    {
        return false;
    }

    public ILootData CloneData()
    {
        return new SkillTreeLootData { statKey = statKey, statModifier = statModifier };
    }

    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() { new LootParams(ELootType.Stat, CloneData()) };
    }

    public string GetParams()
    {
        return "";
    }

    public void Loot()
    {
    }
    public string GetDescription()
    {
        return new AttributeStatModifier(statKey, statModifier).ToString();
    }
}