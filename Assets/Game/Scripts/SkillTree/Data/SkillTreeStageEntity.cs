using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class SkillTreeStageEntity
{
    public int Level;
    public int Index;
    public bool MilestoneEnd;

    public AttributeStatModifier Modifier;
    public ResourceData Cost;


    public SkillTreeStageEntity(BGEntity e)
    {
        MilestoneEnd = false;
        Level = e.Get<int>("Level");
        Index = e.Get<int>("Index");
        Enum.TryParse(e.Get<string>("Stat"), out StatKey Stat);
        Enum.TryParse(e.Get<string>("StatMod"), out EStatMod Mod);
        var Value = e.Get<float>("Value");

        Modifier = new AttributeStatModifier(Stat, new StatModifier(Mod, Value));

        Enum.TryParse(e.Get<string>("CostType"), out EResource CostType);
        var CostValue = e.Get<int>("CostValue");

        Cost = new ResourceData { Value = CostValue, Resource = CostType };
    }
}