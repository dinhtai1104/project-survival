using Foundation.Game.Time;
using System;

[System.Serializable]
public class ChestBaseSave
{
    public EChest Type;
    public int OpenCount;
    public DateTime TimeLastOpenAd;
    public int AdLimit;
    public int OpenTime = 0;

    public void OpenChest()
    {
        OpenTime++;
        Save();
    }
    public ChestBaseSave()
    {
        OpenCount = 0;
        TimeLastOpenAd = DateTime.MinValue;
    } 
    public ChestBaseSave(EChest Type) : this()
    {
        this.Type = Type;
    }
    public void SetLastTimeOpenAd(DateTime TimeLastOpenAd)
    {
        this.TimeLastOpenAd = TimeLastOpenAd;
        Save();
    }

    public void NextDay(int adLimit)
    {
        AdLimit = adLimit;
    }

    public void Open()
    {
        OpenCount++;
        Save();
    }

    public void Save()
    {
        DataManager.Save.Chest.Save();
    }

    public bool CanOpenGem()
    {
        var db = DataManager.Base.Chest.Get(Type);
        if (DataManager.Save.Resources.HasResource(db.CurrencyCost))
        {
            return true;
        }
        return false;
    }

    public bool CanOpenKey()
    {
        var db = DataManager.Base.Chest.Get(Type);
        if (DataManager.Save.Resources.HasResource(db.KeyCost))
        {
            return true;
        }
        return false;
    }

    public bool CanOpenFreeAd()
    {
        var db = DataManager.Base.Chest.Get(Type);
        if (db.TimeRewardAd == -1) return false;
        if ((UnbiasedTime.UtcNow - TimeLastOpenAd).TotalSeconds >= db.TimeRewardAd)
        {
            if (AdLimit > 0 || AdLimit == -1)
            {
                return true;
            }
        }
        return false;
    }
}