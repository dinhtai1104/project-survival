using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class DailyQuestProgressTable : DataTable<int, DailyQuestProgressEntity>
{
    public int TotalScore;
    public override void GetDatabase()
    {
        DB_DailyQuestProgress.ForEachEntity(e => Get(e));
        TotalScore = Dictionary[Dictionary.Count - 1].Milestone;
    }

    private void Get(BGEntity e)
    {
        var entity = new DailyQuestProgressEntity(e);
        Dictionary.Add(entity.Id, entity);
    }
}
