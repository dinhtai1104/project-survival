using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AchievementQuestSaves : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<EAchievement, AchievementQuestSave> Saves;

    public AchievementQuestSaves(string key) : base(key)
    {
        Saves = new Dictionary<EAchievement, AchievementQuestSave>();
        var database = DataManager.Base.AchievementQuest;
        foreach (var type in (EAchievement[])Enum.GetValues(typeof(EAchievement)))
        {
            if (!Saves.ContainsKey(type))
            {
                var model = database.Get(type);
                if (model != null)
                {
                    Saves.Add(type, new AchievementQuestSave(type));
                }
            }
        }
    }

    public override void Fix()
    {
        var database = DataManager.Base.AchievementQuest;
        foreach (var type in (EAchievement[])Enum.GetValues(typeof(EAchievement)))
        {
            if (!Saves.ContainsKey(type))
            {
                var model = database.Get(type);
                if (model != null)
                {
                    Saves.Add(type, new AchievementQuestSave(type));
                }
            }
        }

        var notFound = Saves.Where(t =>
        {
            var e = database.Get(t.Key);
            if (e == null)
            {
                return true;
            }
            return false;
        }).ToArray();

        foreach (var item in notFound)
        {
            Saves.Remove(item.Key);
        }
    }

    public void SetProgress(EAchievement type, int progress)
    {
        if (!Saves.ContainsKey(type)) return;
        Saves[type].SetProgress(progress);
        Save();
    }

    public void IncreaseProgress(EAchievement type)
    {
        if (!Saves.ContainsKey(type)) return;
        Saves[type].Progress++;
        Save();
    }

    public void ReceiveAchievement(EAchievement type)
    {
        if (!Saves.ContainsKey(type)) return;
        Saves[type].Received++;
        Save();
    }

    public bool IsReceiveAchievementIndex(EAchievement type, int Index)
    {
        return Saves[type].Received > Index;
    }

    public AchievementQuestSave GetSave(EAchievement type)
    {
        return Saves[type];
    }
    public bool CanReceive()
    {
        foreach (var save in Saves)
        {
            if (CanReceive(save.Key)) return true;
        }
        return false;
    }
    public bool CanReceive(EAchievement type)
    {
        var database = DataManager.Base.AchievementQuest;
        var data = database.Get(type);
        if (data == null) return false;
        var currentProgress = Saves[type].Progress;

        foreach (var model in data.Achievements)
        {   
            if (currentProgress >= model.Target)
            {
                if (!IsReceiveAchievementIndex(model.Type, model.Index))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsLootAll(EAchievement type)
    {

        var database = DataManager.Base.AchievementQuest;

        var data = database.Get(type);
        return Saves[type].Received >= data.Achievements.Count;
    }
}
