using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;

public class ExpData : ILootData
{
    public double ValueLoot => Exp;
    public long Exp;
    public bool CanMergeData => true;
    public ExpData() { }
    public ExpData(List<string> Param)
    {
        long.TryParse(Param[0], out Exp);
    }

    public bool Add(ILootData data)
    {
        var exp = data as ExpData;
        if (exp != null)
        {
            Exp += (data as ExpData).Exp;
            return true;
        }
        return false;
    }
    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() { new LootParams(ELootType.Exp, this) };
    }
    public void Loot()
    {
        DataManager.Save.User.AddExp(Exp);
        Architecture.Get<BattlePassService>().AddExp(Exp);
    }
    ILootData ILootData.CloneData()
    {
        return new ExpData { Exp = Exp };
    }

    public void Multiply(float v)
    {
        Exp = (long)(Exp * v);
    }

    public string GetParams()
    {
        return $"Exp;{Exp}";
    }
}