using System;
using System.Collections.Generic;

[System.Serializable]
public class HeroData : ILootData
{
    public double ValueLoot => 1;
    public bool CanMergeData => false;
    public EHero HeroType;

    public HeroData() { }

    public HeroData(List<string> datas) : this()
    {
        Enum.TryParse(datas[0], out HeroType);
    }

    public bool Add(ILootData data)
    {
        return false;
    }
    public void Loot()
    {
        DataManager.Save.User.UnlockHero(HeroType);
    }

    public List<LootParams> GetAllData()
    {
        if (DataManager.Save.User.IsUnlockHero(HeroType))
        {
            return new List<LootParams>() { new LootParams(ELootType.HeroFragment, new ResourceData { Value = 20, Resource = EResource.NormalHero + (int)HeroType }) };
        }
        return new List<LootParams>() { new LootParams(ELootType.Hero, this) };
    }

    ILootData ILootData.CloneData()
    {
        return new HeroData { HeroType = HeroType };
    }

    public string GetParams()
    {
        return $"Hero;{HeroType}";
    }
}
