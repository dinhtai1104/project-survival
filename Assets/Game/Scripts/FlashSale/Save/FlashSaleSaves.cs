using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FlashSaleSaves : BaseDatasave
{
    [ShowInInspector] public Dictionary<int, FlashSaleSave> Saves = new Dictionary<int, FlashSaleSave>();
    public bool ShowStarterPack = false;
    public FlashSaleSaves(string key) : base(key)
    {
        Saves = new Dictionary<int, FlashSaleSave>();
        var db = DataManager.Base.FlashSale;
        foreach (var model in db.Dictionary)
        {
            if (Saves.ContainsKey(model.Value.Id) == false)
            {
                Saves.Add(model.Value.Id, new FlashSaleSave(model.Value.Id, model.Value.Type));
            }
        }
    }

    public override void Fix()
    {
        var db = DataManager.Base.FlashSale;
        bool save = false;
        foreach (var model in db.Dictionary)
        {
            if (Saves.ContainsKey(model.Value.Id) == false)
            {
                save = true;
                Saves.Add(model.Value.Id, new FlashSaleSave(model.Value.Id, model.Value.Type));
            }
        }
        var notFound = Saves.Where(t => db.Get(t.Value.Id) == null).ToArray();
        foreach (var model in notFound)
        {
            Saves.Remove(model.Value.Id);
        }
        if (save || notFound.Length > 0)
        {
            Save();
        }
    }

    public FlashSaleSave GetSave(int id)
    {
        return Saves[id];
    }

    public FlashSaleSave GetSave(EFlashSale sale)
    {
        foreach (var model in Saves)
        {
            if (model.Value.Sale == sale) return model.Value;
        }
        return null;
    }

    public void ActiveSale(EFlashSale type)
    {
        var save = GetSave(type);
        if (save == null)
        {
            Logger.Log("Not Found Flash Sale: " + type + " In Save Data");
            return;
        }
        save.Active();
    } 

    public void ClaimFlashSale(EFlashSale type)
    {
        var save = GetSave(type);
        if (save == null)
        {
            Logger.Log("Not Found Flash Sale: " + type + " In Save Data");
            return;
        }
        save.IsClaimed = true;
        Save();
    }

    public void Show(EFlashSale type)
    {
        var save = GetSave(type);
        if (save == null)
        {
            Logger.Log("Not Found Flash Sale: " + type + " In Save Data");
            return;
        }
        save.Show();
        Save();
    }

    public bool IsMaxShowInDay(EFlashSale type)
    {
        var save = GetSave(type);
        if (save == null)
        {
            Logger.Log("Not Found Flash Sale: " + type + " In Save Data");
            return false;
        }
        var db = DataManager.Base.FlashSale;
        var e = db.Get(type);

        return save.ShowInDay >= e.MaxShowInDay;
    }

    public override void NextDay()
    {
        base.NextDay();
        foreach (var save in Saves)
        {
            save.Value.NextDay();
        }
        Save();
    }
}
