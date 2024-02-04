using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DungeonEventSaves : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<EDungeonEvent, DungeonEventSave> Saves;

    public DungeonEventSaves(string key) : base(key)
    {
        Saves = new Dictionary<EDungeonEvent, DungeonEventSave>();
        var allEvent = (EDungeonEvent[])Enum.GetValues(typeof(EDungeonEvent));

        foreach (var ev in allEvent)
        {
            if (ev == EDungeonEvent.None) break;
            if (Saves.ContainsKey(ev) == false)
            {
                var evenData = new DungeonEventSave(ev);
                Saves.Add(ev, evenData);
            }
        }
    }
    public override void NextDay()
    {
        base.NextDay();
        foreach (var ev in Saves)
        {
            ev.Value.NextDay();
        }
    }
    public override void Fix()
    {
        var allEvent = (EDungeonEvent[])Enum.GetValues(typeof(EDungeonEvent));

        foreach (var ev in allEvent)
        {
            if (ev == EDungeonEvent.None) break;
            if (Saves.ContainsKey(ev) == false)
            {
                var evenData = new DungeonEventSave(ev);
                evenData.Type = ev;
                Saves.Add(ev, evenData);
            }
        }
    }
}
