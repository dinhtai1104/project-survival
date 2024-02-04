using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class PiggyBankSave
{
    public int Id;
    public Dictionary<EPiggyBank, bool> Claimed;
    [ShowInInspector]
    public DateTime TimeEnd; // Time reset
    public PiggyBankSave(int id)
    {
        Claimed = new Dictionary<EPiggyBank, bool>()
        {
            { EPiggyBank.FREE, false },
            { EPiggyBank.AD, false },
            { EPiggyBank.PURCHASE, false },
        };
        Id = id;
    }

    public void Active(DateTime time)
    {
        TimeEnd = time;
        Claimed = new Dictionary<EPiggyBank, bool>()
        {
            { EPiggyBank.FREE, false },
            { EPiggyBank.AD, false },
            { EPiggyBank.PURCHASE, false },
        };
    }

    public void Claim(EPiggyBank type)
    {
        Claimed[type] = true;
    }

    public bool IsClaim(EPiggyBank type)
    {
        return Claimed[type];
    }

    public bool IsClaimFull()
    {
        foreach (var type in Claimed)
        {
            if (type.Value == false) return false;
        }
        return true;
    }

    public bool IsEnd()
    {
        return (TimeEnd - UnbiasedTime.UtcNow).TotalSeconds <= 0;
    }
}