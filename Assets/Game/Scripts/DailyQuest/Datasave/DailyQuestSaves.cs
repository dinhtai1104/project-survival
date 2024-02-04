using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Android;

[System.Serializable]
public class DailyQuestSaves : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<int, DailyQuestSave> Saves;
    [ShowInInspector]
    public Dictionary<int, bool> Milestones;
    public int Medal;
    public DailyQuestSaves(string key) : base(key)
    {
        Clear();
    }

    private void Clear()
    {
        Saves = new Dictionary<int, DailyQuestSave>();
        Milestones = new Dictionary<int, bool>();
        var data = DataManager.Base.DailyQuest;
        foreach (var mission in data.Dictionary)
        {
            if (!Saves.ContainsKey(mission.Key))
            {
                Saves.Add(mission.Key, new DailyQuestSave { Id = mission.Key, Mission = mission.Value.Type, Received = false });
            }
        }
        Milestones = new Dictionary<int, bool>();
        var dataProgress = DataManager.Base.DailyQuestProgress;
        foreach (var model in dataProgress.Dictionary)
        {
            Milestones.Add(model.Key, false);
        }
        Medal = 0;
    }

    public override void NextDay()
    {
        base.NextDay();
        Clear();
        IncreaseProgress(EMissionDaily.Login);
    }

    public override void Fix()
    {
        var data = DataManager.Base.DailyQuest;
        foreach (var mission in data.Dictionary)
        {
            if (!Saves.ContainsKey(mission.Key))
            {
                Saves.Add(mission.Key, new DailyQuestSave { Id = mission.Key, Mission = mission.Value.Type, Received = false });
            }
        }

        var notFound = Saves.Where(t =>
        {
            var e = data.Get(t.Key);
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


        if (Milestones == null)
        {
            Milestones = new Dictionary<int, bool>();
            var dataProgress = DataManager.Base.DailyQuestProgress;
            foreach (var model in dataProgress.Dictionary)
            {
                Milestones.Add(model.Key, false);
            }
        }
    }

    public bool CanReceive(int Id)
    {
        if (!Saves.ContainsKey(Id)) return false;
        return Saves[Id].Progress >= DataManager.Base.DailyQuest.Get(Id).Target && Saves[Id].Received == false;
    }

    public bool CanReceiveMilestone(int Id)
    {
        return Medal >= DataManager.Base.DailyQuestProgress.Get(Id).Milestone && Milestones[Id] == false;
    }
    public void ReceiveMilestone(int Id)
    {
        Milestones[Id] = true;
        Save();
    }

    public bool IsReceiveMilestone(int Id)
    {
        return Milestones[Id];
    }

    public bool IsReceiveMission(int Id)
    {
        if (!Saves.ContainsKey(Id)) return false;
        return Saves[Id].Received;
    }

    public void ReceiveMission(int Id)
    {
        if (!Saves.ContainsKey(Id)) return;
        Saves[Id].Receive();
        Medal += DataManager.Base.DailyQuest.Get(Id).Score;
        Save();
    }

    public DailyQuestSave FindMission(EMissionDaily eMission)
    {
        var find = Saves.Where(t => t.Value.Mission == eMission);
        if (find == null || find.Count() == 0) return null;
        return find.ToList()[0].Value;
    }

    public void IncreaseProgress(EMissionDaily eMission)
    {
        var daily = FindMission(eMission);
        if (daily == null) return;
        daily.Progress++;
        Save();
    }

    public bool CanReceive()
    {
        foreach (var save in Milestones)
        {
            if (CanReceiveMilestone(save.Key)) return true;
        }
        foreach (var save in Saves)
        {
            var daily = DataManager.Base.DailyQuest.Get(save.Value.Mission);
            if (daily == null) continue;
            if (daily.Target <= save.Value.Progress && !save.Value.Received) return true;
        }
        return false;
    }
}
