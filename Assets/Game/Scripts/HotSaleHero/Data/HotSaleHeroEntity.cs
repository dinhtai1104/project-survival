using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class HotSaleHeroEntity
{
    public int Id;
    public EHero Hero;
    public List<LootParams> Rewards = new List<LootParams>();
    public string ProductId;
    public int SaleOff;
    public TimeSpan Time;

    public HotSaleHeroEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        SaleOff = e.Get<int>("SaleOff");
        Enum.TryParse(e.Get<string>("Hero"), out Hero);
        var datas = e.Get<List<string>>("Reward");

        foreach (var data in datas)
        {
            var loot = new LootParams(data);
            Rewards.Add(loot);
        }

        ProductId = e.Get<string>("ProductId");
        Time = new TimeSpan(0, 0, e.Get<int>("Time"));
    }
}