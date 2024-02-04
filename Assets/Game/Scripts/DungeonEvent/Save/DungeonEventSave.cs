using System;

[System.Serializable]
public class DungeonEventSave : DungeonSave
{
    public EDungeonEvent Type;
    public int AdLeftDay;
    public int FreeLeftDay;
    public int MaxStage;

    public DungeonEventSave() : base()
    {
        CurrentDungeon = 0;
        MaxStage = 0;
    }

    public DungeonEventSave(EDungeonEvent type) : this()
    {
        Type = type;
        AdLeftDay = DataManager.Base.DungeonEventConfig.Get(type)[CurrentDungeon].MaxAdInDay;
        FreeLeftDay = DataManager.Base.DungeonEventConfig.Get(type)[CurrentDungeon].FreePlay;
    }

    public override void NextDay()
    {
        base.NextDay();
        AdLeftDay = DataManager.Base.DungeonEventConfig.Get(Type)[CurrentDungeon].MaxAdInDay;
        FreeLeftDay = DataManager.Base.DungeonEventConfig.Get(Type)[CurrentDungeon].FreePlay;       
    }
    public override void Save()
    {
        DataManager.Save.DungeonEvent.Save();
    }
}