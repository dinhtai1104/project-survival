using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class SubscriptionEntity
{
    public int Id;
    public ESubscription Type;
    public LootParams Reward;
    public LootParams RewardDaily;
    public float ExtraGoldDungeon;
    public float ExtraExpDungeon;
    public bool RemoveAdReward;
    public string ProductId;
    public int Time;
    public int Energy;

    public SubscriptionEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Enum.TryParse(e.Get<string>("Type"), out Type);
        Reward = new LootParams(e.Get<string>("Reward"));
        RewardDaily = new LootParams(e.Get<string>("RewardDaily"));
        ExtraGoldDungeon = e.Get<float>("ExtraGoldDungeon");
        ExtraExpDungeon = e.Get<float>("ExtraExpDungeon");
        RemoveAdReward = e.Get<int>("RemoveAdReward") == 1;
        Time = e.Get<int>("Time");
        ProductId = e.Get<string>("ProductId");
        Energy = e.Get<int>("Energy");
    }
}