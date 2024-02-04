using System;
using System.Collections.Generic;

[System.Serializable]
public class BuffsSave
{
    public DungeonBuffData Dungeon;
    public bool IsPlaying => Dungeon != null ? Dungeon.IsJoin : false;
    public BuffsSave()
    {
        Dungeon = new DungeonBuffData();
    }

    public void CreateNewGame()
    {
        Dungeon.Clear(false);
        Dungeon.IsJoin = true;
    }

    public void Clear()
    {
        Dungeon.Clear();
    }

    public void RemoveBuff(EBuff buff)
    {
        Dungeon.RemoveBuff(buff);
    }

    public bool HasBuff(List<EBuff> buffConditionForThisBuff)
    {
        return Dungeon.HasBuff(buffConditionForThisBuff);
    }
}
