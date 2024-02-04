using System;
using System.Collections.Generic;

[System.Serializable]
public class DungeonSessionSave : BaseDatasave
{
    /// <summary>
    /// Check game resumed or not?
    /// </summary>
    public GameMode Mode;
    public bool IsAskFirstTime = false;
    public bool IsPlaying = false;
    public int CurrentDungeon;
    public int CurrentStage;
    public List<string> lootData = new List<string>();
    public BuffsSave buffSession;
    public float CurrentHpPercent;
    public int DroneTotalCast = 0;
    public int TotalBossDefeated = 0;
    public DungeonSessionMemoryMap MemoryMap;
    public bool IsRevived = false;
    public bool IsTriedHero;
    public DungeonSessionSave(string key) : base(key)
    {
        Mode = GameMode.Normal;
        lootData = new List<string>();
        buffSession = new BuffsSave();
        CurrentHpPercent = 1;
        IsRevived = false;
    }

    public void SetMemoryMap(DungeonSessionMemoryMap memory)
    {
        MemoryMap = memory;
        Save();
    }
    public void SetHp(float hp)
    {
        this.CurrentHpPercent = hp;
    }

    public void JoinDungeon(int dungeonId)
    {
        IsPlaying = true;
        this.CurrentDungeon = dungeonId;
        Logger.Log("join:" +this.CurrentDungeon);
        Save();
    }
    public void JoinStage(int stageId)
    {
        this.CurrentStage = stageId;
        Save();
    }
    public override void Fix()
    {
        if (this.IsAskFirstTime)
        {
            Clear();
        }
    }

    public void Clear()
    {
        Logger.Log("CLEAR");
        DroneTotalCast = 0;
        TotalBossDefeated = 0;
        CurrentHpPercent = 1;
        CurrentDungeon = 0;
        CurrentStage = 0;
        IsAskFirstTime = false;
        lootData?.Clear();
        IsPlaying = false;
        buffSession?.Clear();
        MemoryMap?.Clear();
        MemoryMap = null;
        IsRevived = false;
        IsTriedHero = false;
        Save();
    }

    public void AddLoot(LootParams loot)
    {
        lootData.Add(loot.Data.GetParams());
    }

    public void SetTriedHero(bool tried)
    {
        IsTriedHero = tried;
        Save();
    }
}
