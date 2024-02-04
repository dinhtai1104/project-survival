using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class DungeonStageTable : DataTable<string, DungeonStageEntity>
{
    public override void GetDatabase()
    {
        DB_DungeonStage.ForEachEntity(e => Get(e));
        DB_DungeonEventStage.ForEachEntity(e => Get(e));
    }

    public DungeonStageEntity GetStages(string stage)
    {
        return Dictionary[stage];
    }

    private void Get(DB_DungeonStage e)
    {
        var entity = new DungeonStageEntity(e);
        Dictionary.Add(entity.Stage, entity);
    }
    private void Get(DB_DungeonEventStage e)
    {
        var entity = new DungeonStageEntity(e);
        Dictionary.Add(entity.Stage, entity);
    }
}