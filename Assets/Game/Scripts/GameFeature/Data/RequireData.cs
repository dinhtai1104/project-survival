using System;
using System.Collections.Generic;

public class RequireData : ILootData
{
    public double ValueLoot => Value;
    public ERequire Require;
    public int Value;
    public bool CanMergeData => false;

    public RequireData() { }
    public RequireData(string data)
    {
        var param = data.Split(';');
        Enum.TryParse(param[1], out Require);
        Value = param[2].TryGetInt();
    }
    public RequireData(List<string> param)
    {
        Enum.TryParse(param[0], out Require);
        Value = param[1].TryGetInt();
    }

    public bool Add(ILootData data)
    {
        return false;
    }

    public ILootData CloneData()
    {
        return this;
    }

    public List<LootParams> GetAllData()
    {
        return new List<LootParams>() { new LootParams { Type = ELootType.Require, Data = this } };
    }

    public void Loot()
    {
    }

    public string GetParams()
    {
        return "";
    }
}