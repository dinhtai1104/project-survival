using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ChestEntity
{
    public EChest Type;
    public ResourceData CurrencyCost;
    public ResourceData KeyCost;
    public int TimeRewardAd;
    public int MaxRewardDay;


    public Dictionary<EChestReward, List<ChestRewardBaseData>> Rewards = new Dictionary<EChestReward, List<ChestRewardBaseData>>();

    public ChestEntity() { }

    public void AddRow(ChestEquipmentRow chestRow)
    {
        var rewardType = chestRow.Type;
        if (!Rewards.ContainsKey(rewardType))
        {
            Rewards.Add(rewardType, new List<ChestRewardBaseData>());
        }
        Rewards[rewardType].Add(chestRow.Reward);
    }
}