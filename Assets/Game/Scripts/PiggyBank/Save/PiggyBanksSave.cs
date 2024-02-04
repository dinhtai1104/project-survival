using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PiggyBanksSave : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<int, PiggyBankSave> Saves;
    public int PiggyCurrent = 0;
    public PiggyBanksSave(string key) : base(key)
    {
        Saves = new Dictionary<int, PiggyBankSave>();
        var db = DataManager.Base.PiggyBank;
        foreach (var model in db.Dictionary)
        {
            if (!Saves.ContainsKey(model.Key))
            {
                Saves.Add(model.Key, new PiggyBankSave(model.Key));
            }
        }
        Saves[0].Active(UnbiasedTime.UtcNow + db.Get(PiggyCurrent).TimeReset);
    }

    public override void Fix()
    {
        var db = DataManager.Base.PiggyBank;
        foreach (var model in db.Dictionary)
        {
            if (!Saves.ContainsKey(model.Key))
            {
                Saves.Add(model.Key, new PiggyBankSave(model.Key));
            }
        }

        if (Saves[PiggyCurrent].IsClaimFull())
        {
            Active(PiggyCurrent + 1, true);
        }

        if (PiggyCurrent >= db.Dictionary.Count)
        {
            PiggyCurrent = Mathf.Clamp(PiggyCurrent, 0, db.Dictionary.Count - 1);
            Active(PiggyCurrent, true);
        }

        if (Saves[PiggyCurrent].IsEnd())
        {
            Active(PiggyCurrent, true);
        }
    }

    public void Active(int Id, bool IsReset = false, bool init = false)
    {
        var db = DataManager.Base.PiggyBank;
        var id = Mathf.Clamp(Id, 0, db.Dictionary.Count - 1);
        var target = db.Get(id).Target;
        if (init == false)
        {
            DataManager.Save.Resources.DecreaseResource(new ResourceData(EResource.PigCoin, target));
        }
        if (IsReset)
        {
            DataManager.Save.Resources.DecreaseResource(new ResourceData(EResource.PigCoin, DataManager.Save.Resources.GetResource(EResource.PigCoin)));
        }
        Id = Mathf.Clamp(Id, 0, db.Dictionary.Count - 1);
        PiggyCurrent = Id;

        Saves[Id].Active(UnbiasedTime.UtcNow + db.Get(PiggyCurrent).TimeReset);
        Save();
    }

    public void ClaimPiggy(EPiggyBank type)
    {
        Saves[PiggyCurrent].Claim(type);
        if (Saves[PiggyCurrent].IsClaimFull())
        {
            var db = DataManager.Base.PiggyBank;
            PiggyCurrent++;
            if (PiggyCurrent >= db.Dictionary.Count)
            {
                PiggyCurrent = Mathf.Clamp(PiggyCurrent, 0, db.Dictionary.Count - 1);
                Active(PiggyCurrent, true);
            }
            else
            {
                Active(PiggyCurrent);
            }
        }
        Save();
    }
    public bool IsClaim(EPiggyBank type, int id)
    {
        return Saves[id].IsClaim(type);
    }

    public bool CanClaim()
    {
        var db = DataManager.Base.PiggyBank;
        var target = db.Get(PiggyCurrent).Target;
        var res = DataManager.Save.Resources.GetResource(EResource.PigCoin);
        if (res >= target)
        {
            if (!IsClaim(EPiggyBank.FREE, PiggyCurrent))
            {
                return true;
            }
            if (!IsClaim(EPiggyBank.AD, PiggyCurrent))
            {
                return true;
            }
            if (!IsClaim(EPiggyBank.PURCHASE, PiggyCurrent))
            {
                return true;
            }
        }
        return false;
    }
}
