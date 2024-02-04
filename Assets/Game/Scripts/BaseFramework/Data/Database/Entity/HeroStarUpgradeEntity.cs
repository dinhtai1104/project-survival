using System;

[System.Serializable]
public class HeroStarUpgradeEntity
{
    public EHero Hero;
    public int Fragment;
    public int Star;

    public ResourceData HeroFragment;

    public float HpBaseAdd;
    public float DmgBaseAdd;

    public float HpGrownAdd;
    public float DmgGrownAdd;

    public AttributeStatModifier Reward;

    public HeroStarUpgradeEntity(DB_HeroStarUpgrade e)
    {
        Enum.TryParse(e.Get<string>("Hero"), out Hero);
        var heroEntity = DataManager.Base.Hero.GetHero(Hero);
        
        Star = e.Get<int>("Star");
        Fragment = e.Get<int>("Fragment");
        //var rewardData = e.Get<string>("Reward");
        //Reward = new AttributeStatModifier(rewardData);

        HpBaseAdd = e.Get<float>("HpBase");
        DmgBaseAdd = e.Get<float>("DmgBase");

        HpGrownAdd = e.Get<float>("HpGrown");
        DmgGrownAdd = e.Get<float>("DmgGrown");
        HeroFragment = new ResourceData { Resource = heroEntity.HeroResource, Value = Fragment };
    }
}