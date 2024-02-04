using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class ChestEquipmentRow
{
    public EChestReward Type;
    public ChestRewardBaseData Reward;

    public ChestEquipmentRow(BGEntity e)
    {
        Enum.TryParse(e.Get<string>("RewardType"), out Type);

        switch (Type)
        {
            case EChestReward.Equipment:
                Reward = new ChestEquipmentRewardData(e);
                Reward.Type = ELootType.Equipment;
                break;
            case EChestReward.Currency:
                Reward = new ChestResourceRewardData(e);
                Reward.Type = ELootType.Resource;
                break;
            case EChestReward.Fragment:
                Reward = new ChestResourceRewardData(e);
                Reward.Type = ELootType.Fragment;
                break;
            case EChestReward.HeroFragment:
                Reward = new ChestResourceRewardData(e);
                Reward.Type = ELootType.HeroFragment; 
                break;
            case EChestReward.HeroStone:
                Reward = new ChestResourceRewardData(e);
                Reward.Type = ELootType.HeroStone; 
                break;
        }
    }
}