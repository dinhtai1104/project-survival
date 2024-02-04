using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ChestsSave : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<EChest, ChestBaseSave> Saves;

    public ChestsSave(string key) : base(key)
    {
        Saves = new Dictionary<EChest, ChestBaseSave>();

        foreach (var eChest in (EChest[])Enum.GetValues(typeof(EChest)))
        {
            if (!Saves.ContainsKey(eChest))
            {
                Saves.Add(eChest, new ChestBaseSave(eChest));
                Saves[eChest].AdLimit = DataManager.Base.Chest.Get(eChest)?.MaxRewardDay ?? 0;
            }
        }
    }

    public override void Fix()
    {
        foreach (var eChest in (EChest[])Enum.GetValues(typeof(EChest)))
        {
            if (!Saves.ContainsKey(eChest))
            {
                Saves.Add(eChest, new ChestBaseSave(eChest));
            }
        }
    }
    public override void NextDay()
    {
        base.NextDay();
        foreach (var chest in Saves)
        {
            var entity = DataManager.Base.Chest.Get(chest.Key);
            if (entity != null)
            {
                chest.Value.NextDay(entity.MaxRewardDay);
            }
        }
    }
}
