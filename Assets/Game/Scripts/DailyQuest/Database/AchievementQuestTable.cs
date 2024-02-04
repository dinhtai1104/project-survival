using System;

[System.Serializable]
public class AchievementQuestTable : DataTable<EAchievement, AchievementQuestEntity>
{
    public override void GetDatabase()
    {
        DB_Achievement.ForEachEntity(e => Get(e));
    }

    private void Get(DB_Achievement e)
    {
        Enum.TryParse(e.Get<string>("Type"), out EAchievement Type);
        if (!Dictionary.ContainsKey(Type))
        {
            var entity = new AchievementQuestEntity(e);
            Dictionary.Add(Type, entity);
        }
        var stage = new AchievementStageEntity(e);
        if (Dictionary.ContainsKey(Type))
        {
            Dictionary[Type].AddAchievement(stage);
        }
    }
}
