using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DungeonEventConfigTable : DataTable<EDungeonEvent, Dictionary<int, DungeonEventConfigEntity>>
{
    public override void GetDatabase()
    {
        DB_DungeonEventConfig.ForEachEntity(e => Get(e));
    }

    private void Get(BGEntity e)
    {
        var cf = new DungeonEventConfigEntity(e);
        if (!Dictionary.ContainsKey(cf.Event))
        {
            Dictionary.Add(cf.Event, new Dictionary<int, DungeonEventConfigEntity>());
        }
        Dictionary[cf.Event].Add(cf.Dungeon, cf);
    }
}
