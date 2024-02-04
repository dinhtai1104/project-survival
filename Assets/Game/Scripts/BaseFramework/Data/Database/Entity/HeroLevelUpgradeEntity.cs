using System;

[System.Serializable]
public class HeroLevelUpgradeEntity
{
    public EHero Hero;
    public int Level;
    public ResourceData Fragment;
    public ResourceData Cost;
    public AttributeStatModifier Reward;
    public bool Milestone = false;
    public bool IsUseForAllHero = false;

    public HeroLevelUpgradeEntity(DB_HeroLevelUpgrade e)
    {
        Enum.TryParse(e.Get<string>("Hero"), out Hero);
        Level = e.Get<int>("Level");

        //Stone
        var stoneValue = e.Get<int>("StoneValue");
        var heroStone = DataManager.Base.Hero.GetHero(Hero).StoneResource;
        Fragment = new ResourceData { Value = stoneValue, Resource = heroStone };

        //Cost
        var costValue = int.Parse(e.Get<string>("CostValue"));
        Enum.TryParse(e.Get<string>("CostType"), out EResource costType);
        Cost = new ResourceData { Resource = costType, Value = costValue };


        var rewardData = e.Get<string>("Reward");
        Milestone = false;

        if (!string.IsNullOrEmpty(rewardData))
        {
            Milestone = true;
            Reward = new AttributeStatModifier(rewardData);
        }
        IsUseForAllHero = e.Get<int>("IsUseForAllHero") == 1;
    }
}