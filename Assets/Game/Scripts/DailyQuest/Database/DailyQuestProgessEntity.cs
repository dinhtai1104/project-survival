using BansheeGz.BGDatabase;
using System.Collections.Generic;

[System.Serializable]
public class DailyQuestProgressEntity
{
    public int Id;
    public int Milestone;
    public List<LootParams> Reward = new List<LootParams>();
    public bool IsSpecial;

    public DailyQuestProgressEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Milestone = e.Get<int>("Milestone");
        foreach (var data in e.Get<List<string>>("Reward"))
        {
            Reward.Add(new LootParams(data));
        }
        IsSpecial = e.Get<int>("IsSpecial") == 1;
    }
}