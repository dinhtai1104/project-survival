using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class DungeonEventConfigEntity
{
    public EDungeonEvent Event;
    public EResource Resource;
    public int MaxAdInDay;
    public int FreePlay;
    public int Energy;
    public bool IsActive;
    public int Base;
    public float Increase;

    public ILootData StartReward;
    public ILootData IncreaseReward;

    public float EnemyLevel;
    public int Dungeon;
    public DungeonEventConfigEntity(BGEntity e)
    {
        Enum.TryParse(e.Get<string>("Event"), out Event);
        Enum.TryParse(e.Get<string>("Reward"), out Resource);
        MaxAdInDay = e.Get<int>("MaxAdInDay");
        FreePlay = e.Get<int>("FreePlay");
        Energy = e.Get<int>("Energy");
        IsActive = e.Get<int>("Active") == 1;
        Base = e.Get<int>("Base");
        Increase = e.Get<float>("Increase");

        StartReward = new ResourceData { Resource = Resource, Value = Base };
        IncreaseReward = new ResourceData { Resource = Resource, Value = (long)(Increase * Base) };

        Dungeon = e.Get<int>("Dungeon");
        EnemyLevel = e.Get<float>("EnemyLevel");
    }
}