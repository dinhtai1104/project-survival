using Assets.Game.Scripts.BaseFramework.Architecture;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SubscriptionSaves : BaseDatasave
{
    public Dictionary<ESubscription, SubscriptionSave> Saves;
    public bool CanReward = false;
    public SubscriptionSaves(string key) : base(key)
    {
        Saves = new Dictionary<ESubscription, SubscriptionSave>();
        var db = DataManager.Base.Subscription;
        foreach (var model in db.Dictionary)
        {
            if (!Saves.ContainsKey(model.Value.Type))
            {
                Saves.Add(model.Value.Type, new SubscriptionSave(model.Value.Id, model.Value.Type));
            }
        }
    }
    public override void NextDay()
    {
        base.NextDay();
        CanReward = true;
    }

    public void Reward()
    {
        CanReward = false;
        Save();
    }

    public override void OnLoaded()
    {
        base.OnLoaded();
        ApplySubscription();
    }

    public override void Fix()
    {
        var db = DataManager.Base.Subscription;
        foreach (var model in db.Dictionary)
        {
            if (!Saves.ContainsKey(model.Value.Type))
            {
                Saves.Add(model.Value.Type, new SubscriptionSave(model.Value.Id, model.Value.Type));
            }
            Saves[model.Value.Type].Save = Save;
        }

        foreach (var save in Saves)
        {
            save.Value.Check();
        }
    }

    public void ApplySubscription()
    {
        // Add modifier
        var db = DataManager.Base.Subscription;
        Architecture.Get<EnergyService>().RemoveAllModifier(this);
        foreach (var save in Saves)
        {
            if (save.Value.IsActived)
            {
                var entity = db.Dictionary[save.Value.Id];
                Architecture.Get<EnergyService>().AddCapacityModifier(new StatModifier(EStatMod.Flat, entity.Energy, this));
            }
        }
    }


    public void ActiveSubscription(ESubscription sub)
    {
        Saves[sub].Active();
    }

    public bool CanRewardDaily()
    {
        return CanReward && GetAllDailyRewards().Count > 0;
    }


    public bool IsFreeRewardAd()
    {
        return Saves[ESubscription.Vip].IsActived;
    }

    public float GetExtraGoldDungeon()
    {
        float res = 1;
        var db = DataManager.Base.Subscription;
        foreach (var s in Saves)
        {
            if (s.Value.IsActived)
            {
                res += db.Get(s.Value.Id).ExtraGoldDungeon;
            }
        }
        return res;
    }
    public float GetExtraExpDungeon()
    {
        float res = 1;
        var db = DataManager.Base.Subscription;
        foreach (var s in Saves)
        {
            if (s.Value.IsActived)
            {
                res += db.Get(s.Value.Id).ExtraExpDungeon;
            }
        }
        return res;
    }
    public List<LootParams> GetAllDailyRewards()
    {
        var rw = new List<LootParams>();
        var db = DataManager.Base.Subscription;
        foreach (var s in Saves)
        {
            if (s.Value.IsActived)
            {
                rw.Add(db.Get(s.Value.Id).RewardDaily);
            }
        }

        return rw;
    }
    public List<LootParams> GetDailyRewards(int Id)
    {
        var db = DataManager.Base.Subscription;
        return new List<LootParams>() { db.Get(Id).RewardDaily.Clone() };
    }

    public bool IsActiveAll()
    {
        foreach (var s in Saves)
        {
            if (!s.Value.IsActived)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsActiveAny()
    {
        foreach (var s in Saves)
        {
            if (s.Value.IsActived)
            {
                return true;
            }
        }
        return false;
    }
}
