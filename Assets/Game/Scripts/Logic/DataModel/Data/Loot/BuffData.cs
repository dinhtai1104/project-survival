using System;
using System.Collections.Generic;

public class BuffData : ILootData
{
    public EBuff BuffType;
    public BuffSave DataBuff;
    public BuffData(EBuff buffType)
    {
        BuffType = buffType;
        DataBuff = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped[buffType];
    }

    public double ValueLoot => 1;

    public bool CanMergeData => false;

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
        return new List<LootParams> { };
    }

    public string GetParams()
    {
        return "";
    }

    public void Loot()
    {
    }

    public string Description()
    {
        return I2Localize.GetLocalize($"Buff_Description/{BuffType}");
    }

    public string GetName()
    {
        return I2Localize.GetLocalize($"Buff_Name/{BuffType}");
    }
}