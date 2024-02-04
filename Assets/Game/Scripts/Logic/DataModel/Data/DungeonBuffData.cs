using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DungeonBuffData
{
    public bool IsJoin = false;
    public int DungeonId;
    public int StageId;

    [ShowInInspector]
    public Dictionary<EBuff, BuffSave> BuffEquiped;
    public System.Action Save;

    public DungeonBuffData()
    {
        IsJoin = true;
        BuffEquiped = new Dictionary<EBuff, BuffSave>();
    }

    public void JoinDungeon(int DungeonId)
    {
        IsJoin = true;
        this.DungeonId = DungeonId;
        Save?.Invoke();
    }

    public void JoinStage(int StageId)
    {
        IsJoin = true;
        this.StageId = StageId;
        Save?.Invoke();
    }

    public void Buff(EBuff buffType, int stageBuff)
    {
        if (BuffEquiped.ContainsKey(buffType))
        {
            BuffEquiped[buffType].LevelUp();
        }
        else
        {
            BuffEquiped.Add(buffType, new BuffSave { Level = 1, StageBuff = stageBuff });
        }
        Save?.Invoke();
    }

    public void Clear(bool save = true)
    {
        IsJoin = false;
        BuffEquiped.Clear();
        DungeonId = StageId = -1;
        if (save)
        {
            Save?.Invoke();
        }
    }

    public void RemoveBuff(EBuff buff)
    {
        if (BuffEquiped.ContainsKey(buff))
        {
            BuffEquiped.Remove(buff);
            Save?.Invoke();
        }
    }

    public bool IsMaxLevel(BuffEntity e)
    {
        if (!BuffEquiped.ContainsKey(e.Type)) return false;
        return e.LevelMaxCard <= BuffEquiped[e.Type].Level;
    }

    public bool HasBuff(List<EBuff> buffConditionForThisBuff)
    {
        foreach (var bu in buffConditionForThisBuff)
        {
            if (!BuffEquiped.ContainsKey(bu)) return false;
        }
        return true;
    } 
}