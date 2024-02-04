using System;
using System.Linq;

[System.Serializable]
public class DailyQuestTable : DataTable<int, DailyQuestEntity>
{
    public override void GetDatabase()
    {
        DB_DailyQuest.ForEachEntity(e => Get(e));
    }

    private void Get(DB_DailyQuest e)
    {
        var mission = new DailyQuestEntity(e);
        if (!Dictionary.ContainsKey(mission.Id))
        {
            Dictionary.Add(mission.Id, mission);
        }
    }
    public DailyQuestEntity Get(EMissionDaily daily)
    {
        return Dictionary.Values.ToList().Find(t => t.Type == daily);
    }
}
