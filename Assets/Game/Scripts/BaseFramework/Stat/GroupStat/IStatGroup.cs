using System;
using System.Collections.Generic;

public interface IStatGroup
{
    IEnumerable<StatKey> StatNames { get; }
    IEnumerable<StatModifier> GetModifiers(StatKey statName);
    void AddStat(StatKey statName, float baseValue, float min = 0f, float max = float.MaxValue);
    void AddListener(StatKey statName, Action<float> callback);
    void RemoveListener(StatKey statName, Action<float> callback);

    IStatGroup SetBaseValue(StatKey statName, float value, bool callUpdater = true);
    IStatGroup SetMinValue(StatKey statName, float min);
    IStatGroup SetMaxValue(StatKey statName, float max);
    float GetBaseValue(StatKey statName, float defaultValue = 0f);
    float GetValue(StatKey statName, float defaultValue = 0f);
    float GetLastValue(StatKey statName, float defaultValue = 0f);
    float GetMinConstraint(StatKey statName);
    float GetMaxConstraint(StatKey statName);

    void AddModifier(StatKey statName, StatModifier mod, object source);
    void RemoveModifier(StatKey statName, StatModifier mod);
    void RemoveModifiersFromSource(object source);
    void ClearAllModifiers();
    bool HasStat(StatKey statName);
    bool HasModifier(StatKey statName, StatModifier modifier);

    Stat GetStat(StatKey statName);
    Dictionary<StatKey, Stat> GetAllStat();
    void RemoveStat(StatKey statName);
    void RemoveAllStats();
    void CalculateStats();
    void Copy(IStatGroup refer, float percentage = 1.0f);
    void ReplaceAllStatBySource(IStatGroup refer, object[] source);
}