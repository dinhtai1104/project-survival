using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;

namespace Engine
{
    public interface IStatGroup
    {
        IEnumerable<string> StatNames { get; }
        IEnumerable<StatModifier> GetModifiers(string statName);
        void AddStat(string statName, float baseValue, float min = 0f, float max = float.MaxValue);
        void AddListener(string statName, Action<float> callback);
        void RemoveListener(string statName, Action<float> callback);

        IStatGroup SetBaseValue(string statName, float value, bool callUpdater = true);
        IStatGroup SetMinValue(string statName, float min);
        IStatGroup SetMaxValue(string statName, float max);
        float GetBaseValue(string statName, float defaultValue = 0f);
        float GetValue(string statName, float defaultValue = 0f);
        float GetLastValue(string statName, float defaultValue = 0f);
        float GetMinConstraint(string statName);
        float GetMaxConstraint(string statName);

        void AddModifier(string statName, StatModifier mod, object source);
        void RemoveModifier(string statName, StatModifier mod);
        void RemoveModifiersFromSource(object source);
        void ClearAllModifiers();
        bool HasStat(string statName);
        bool HasModifier(string statName, StatModifier modifier);

        Stat GetStat(string statName);
        Dictionary<string, Stat> GetAllStat();
        void RemoveStat(string statName);
        void RemoveAllStats();
        void CalculateStats();
        void Copy(IStatGroup refer, float percentage = 1.0f);
        void ReplaceAllStatBySource(IStatGroup refer, object[] source);
    }
}