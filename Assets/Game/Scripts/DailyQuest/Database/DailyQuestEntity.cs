using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class DailyQuestEntity
{
    public int Id;
    public EMissionDaily Type;
    public EMissionClaim Claim;
    public int Target;
    public LootParams Reward;
    public int Score;
    
    public DailyQuestEntity(BGEntity e)
    {
        Id = e.Get<int>("Id");
        Enum.TryParse(e.Get<string>("Type"), out Type);
        Enum.TryParse(e.Get<string>("Claim"), out Claim);
        Target = e.Get<int>("Target");

        var data = e.Get<string>("Reward");
        Score = e.Get<int>("Score");
        Reward = new LootParams($"Resource;DailyQuestMedal;{Score}");
    }
}