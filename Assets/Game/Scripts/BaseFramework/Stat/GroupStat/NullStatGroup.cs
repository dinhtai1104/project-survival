using System;
using System.Collections.Generic;

public class NullStatGroup : IStatGroup
{
    public IEnumerable<StatKey> StatNames => null;

    public void AddListener(StatKey statName, Action<float> callback)
    {
    }

    public void AddModifier(StatKey statName, StatModifier mod, object source)
    {
    }

    public void AddStat(StatKey statName, float baseValue, float min = 0, float max = float.MaxValue)
    {
    }

    public void CalculateStats()
    {
    }

    public void ClearAllModifiers()
    {
    }

    public void Copy(IStatGroup refer, float percentage = 1)
    {
    }

    public Dictionary<StatKey, Stat> GetAllStat()
    {
        return null;
    }

    public float GetBaseValue(StatKey statName, float defaultValue = 0)
    {
        return 0;
    }

    public float GetLastValue(StatKey statName, float defaultValue = 0)
    {
        return 0;
    }

    public float GetMaxConstraint(StatKey statName)
    {
        return 0;
    }

    public float GetMinConstraint(StatKey statName)
    {
        return 0;
    }

    public IEnumerable<StatModifier> GetModifiers(StatKey statName)
    {
        return null;
    }

    public Stat GetStat(StatKey statName)
    {
        return null;
    }

    public float GetValue(StatKey statName, float defaultValue = 0)
    {
        return 0;
    }

    public bool HasModifier(StatKey statName, StatModifier modifier)
    {
        return false;
    }

    public bool HasStat(StatKey statName)
    {
        return false;
    }

    public void RemoveAllStats()
    {
    }

    public void RemoveListener(StatKey statName, Action<float> callback)
    {
    }

    public void RemoveModifier(StatKey statName, StatModifier mod)
    {
    }

    public void RemoveModifiersFromSource(object source)
    {
    }

    public void RemoveStat(StatKey statName)
    {
    }

    public void ReplaceAllStatBySource(IStatGroup refer, params object[] source)
    {
    }

    public IStatGroup SetBaseValue(StatKey statName, float value, bool callUpdater = true)
    {
        return this;
    }

    public IStatGroup SetMaxValue(StatKey statName, float max)
    {
        return this;
    }

    public IStatGroup SetMinValue(StatKey statName, float min)
    {
        return this;
    }
}