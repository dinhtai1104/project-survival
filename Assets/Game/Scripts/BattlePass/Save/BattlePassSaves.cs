using Foundation.Game.Time;
using System;
using System.Collections.Generic;

[System.Serializable]
public class BattlePassSaves : BaseDatasave
{
    public bool ActivePremium = false;
    public List<BattlePassSave> battleSave = new List<BattlePassSave>();
    public DateTime LastTimeActive;
    public DateTime TimeEnd;
    public int Season;
    public long Exp;
    public bool CanClaimPremium = false;
    public bool IsRunning = false;
    public BattlePassSaves(string key) : base(key)
    {
        LastTimeActive = DateTime.UtcNow;
        TimeEnd = LastTimeActive.AddSeconds(BattlePassTable.PremiumTime);
    }

    public override void Fix()
    {
        var timeNow = UnbiasedTime.UtcNow;
        if ((TimeEnd - timeNow).TotalSeconds < 0)
        {
            Reset();
        }
    }

    public override void NextDay()
    {
        base.NextDay();
        if (ActivePremium)
        {
            CanClaimPremium = true;
        }
    }

    private void Reset()
    {
        CanClaimPremium = false;
        Season++;
        Exp = 0;
        battleSave.Clear();
        ActivePremium = false;
        LastTimeActive = DateTime.UtcNow;
        TimeEnd = LastTimeActive.AddSeconds(BattlePassTable.PremiumTime);
        IsRunning = false;
        Save();
    }

    public BattlePassSave GetBattlePass(int level)
    {
        var sa = battleSave.Find(t => t.Level == level);
        return sa;
    }

    public BattlePassSave AddBattlePass(int level)
    {
        var bat = new BattlePassSave();
        var data = DataManager.Base.BattlePass.Get(level);

        bat.Id = data.Id;
        bat.Level = data.Level;

        return bat;
    }

    public void BuyBattlePass(int level, EBattlePass type)
    {
        var sa = GetBattlePass(level);
        if (sa == null)
        {
            sa = AddBattlePass(level);
            battleSave.Add(sa);
        }

        switch (type)
        {
            case EBattlePass.Free:
                sa.FreeClaimed = true;
                break;
            case EBattlePass.Premium:
                sa.PremiumClaimed = true;
                break;
        }

        Save();
    }

    public void AddExp(long exp)
    {
        this.Exp += exp;
        Save();
    }

    public bool IsClaimed(int level, EBattlePass type)
    {
        var sa = GetBattlePass(level);
        if (sa == null) return false;
        switch (type)
        {
            case EBattlePass.Free:
                return sa.FreeClaimed;
            case EBattlePass.Premium:
                return sa.PremiumClaimed;
        }
        return false;
    }

    public void BuyPremium()
    {
        ActivePremium = true;
        Save();
    }

    public void ActiveBattlePass()
    {
        IsRunning = true;

        LastTimeActive = DateTime.UtcNow;
        TimeEnd = LastTimeActive.AddSeconds(BattlePassTable.PremiumTime);

        Save();
    }
}