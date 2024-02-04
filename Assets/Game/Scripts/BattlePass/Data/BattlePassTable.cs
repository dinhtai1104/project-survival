using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class BattlePassTable : DataTable<int, BattlePassEntity>
{
    public static string PremiumProductId;
    public static int PremiumTime;
    public override void GetDatabase()
    {
        DB_BattlePass.ForEachEntity(e => Get(e));
    }

    public List<LootParams> GetAllLoot(EBattlePass type)
    {
        var l = new List<LootParams>();

        foreach (var e in Dictionary)
        {
            switch (type)
            {
                case EBattlePass.Free:
                    l.Add(e.Value.FreeReward);
                    break;
                case EBattlePass.Premium:
                    l.Add(e.Value.PremiumReward);
                    break;
            }
        }

        return l;
    }

    public List<long> GetAllExp()
    {
        var totalExp = new List<long>();
        foreach (var e in Dictionary)
        {
            totalExp.Add(e.Value.ExpRequire);
        }
        return totalExp;
    }

    private void Get(DB_BattlePass e)
    {
        var s = e.Get<string>("ProductId");
        var time = e.Get<int>("Time");

        if (s.IsNotNullAndEmpty())
        {
            PremiumProductId = s;
            PremiumTime = time;
        }
        
        var entity = new BattlePassEntity(e);
        if (Dictionary.ContainsKey(entity.Level)) return;
        Dictionary.Add(entity.Level, entity);
    }
}