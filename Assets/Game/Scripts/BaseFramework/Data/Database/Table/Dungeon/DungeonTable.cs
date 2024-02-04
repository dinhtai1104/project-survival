using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DungeonTable : DataTable<int, DungeonEntity>
{
    public override void GetDatabase()
    {
        DB_Dungeon.ForEachEntity(e => Get(e));
    }

    public DungeonEntity GetDungeon(int dungeonId)
    {
        return Dictionary[dungeonId];
    }

    protected virtual void Get(BGEntity e)
    {
        int dungeonId = e.Get<int>("Dungeon");
        int buff = e.Get<int>("Buff");
        if (!Dictionary.ContainsKey(dungeonId))
        {
            Dictionary.Add(dungeonId, new DungeonEntity(dungeonId));
        }
        var stage = e.Get<string>("Stages");
        var stageEntity = DataManager.Base.DungeonStage.GetStages(stage);
        Dictionary[dungeonId].AddStage(stageEntity,buff);

        var RewardStage = new List<LootParams>();
        var rewards = e.Get<List<string>>("Reward");
        if (rewards != null)
        {
            foreach (var data in rewards)
            {
                var rw = new LootParams(data);
                RewardStage.Add(rw);
            }
        }
        stageEntity.RewardStage = RewardStage;
    }
} 