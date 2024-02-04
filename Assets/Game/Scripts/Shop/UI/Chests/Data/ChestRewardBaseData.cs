
using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class ChestRewardBaseData : LootParams, IWeightable
{
    public EChestReward RewardType;
    public float Weight { set; get; }
    public ChestRewardBaseData() : base() { }
    public ChestRewardBaseData(BGEntity e) : this()
    {
        Enum.TryParse(e.Get<string>("RewardType"), out RewardType);
        Weight = e.Get<float>("Weight");
    }
}
