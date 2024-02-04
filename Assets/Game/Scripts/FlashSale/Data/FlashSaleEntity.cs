using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class FlashSaleEntity
{
    public int Id;
    public EFlashSale Type;
    public List<LootParams> Rewards = new List<LootParams>();

    public TimeSpan TimeEndAdd;
    public int Time;
    public int MaxShowInDay;
    public int SaleOff;
    public string ProductId;
    public int XValue;
    
    public FlashSaleEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Enum.TryParse(e.Get<string>("Type"), out Type);
        Time = e.Get<int>("Time");
        MaxShowInDay = e.Get<int>("MaxShowInDay");
        SaleOff = e.Get<int>("SaleOff");
        XValue = e.Get<int>("XValue");
        ProductId = e.Get<string>("ProductId");

        TimeEndAdd = new TimeSpan(0, 0, Time);
        var reward = e.Get<List<string>>("Rewards");
        foreach (var data in reward)
        {
            Rewards.Add(new LootParams(data));
        }
    }
}