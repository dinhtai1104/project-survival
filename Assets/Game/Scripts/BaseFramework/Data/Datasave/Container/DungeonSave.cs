using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

[System.Serializable]
public class DungeonSave : BaseDatasave
{
    public List<int> DungeonCleared;
    public List<int> DungeonShowUnlocked;

    public int CurrentDungeon;
    public int BestStage;

    public DungeonSave()
    {
        DungeonCleared = new List<int>();
        DungeonShowUnlocked = new List<int>();
        
        CurrentDungeon = 0;
        BestStage = 0;
    }

    public DungeonSave(string key) : base(key)
    {
        DungeonCleared = new List<int>();
        DungeonShowUnlocked = new List<int>();
        CurrentDungeon = 0;
        BestStage = 0;
    }

    public void ClearDungeon(int dungeon)
    {
        if (DungeonCleared.Contains(dungeon)) return;
        DungeonCleared.Add(dungeon);
    }
    public override void Fix()
    {
        if (DungeonShowUnlocked == null)
        {
            DungeonShowUnlocked = new List<int>();
            foreach (var i in DungeonCleared)
            {
                DungeonShowUnlocked.Add(i);
            }
            Save();
        }
    }

    public bool IsDungeonCleared(int dungeon)
    {
        return DungeonCleared.Contains(dungeon) || dungeon <= -1;
    }

    public bool CanPlayDungeon(int dungeonId)
    {
        return IsDungeonCleared(dungeonId) || CurrentDungeon >= dungeonId;
    }

    public bool IsShowAnimDungeon(int dungeonId)
    {
        return DungeonShowUnlocked.Contains(dungeonId);
    }
    public void ShowAnimDungeon(int dungeonId)
    {
        if (DungeonShowUnlocked.Contains(dungeonId))
        {
            return;
        }
        DungeonShowUnlocked.Add(dungeonId);
        Save();
    }

    public void ClearNextDungeon()
    {
        int current = CurrentDungeon;
        if (CurrentDungeon < DataManager.Base.Dungeon.Dictionary.Count - 1)
        {
            ClearDungeon(CurrentDungeon);
        }

        current++;
        if (current >= DataManager.Base.Dungeon.Dictionary.Count - 1)
        {
            current = DataManager.Base.Dungeon.Dictionary.Count - 2;
        }

        CurrentDungeon = current;
    }
    public bool IsNewDungeon(int dungeonId)
    {
        return IsDungeonCleared(dungeonId) && !IsShowAnimDungeon(dungeonId);
    }
}
