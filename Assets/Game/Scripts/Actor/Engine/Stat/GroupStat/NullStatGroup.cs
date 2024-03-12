using System;
using System.Collections.Generic;

namespace Engine
{
    public class NullStatGroup : IStatGroup
    {
        public IEnumerable<string> StatNames => null;

        public void AddListener(string statName, StatChangeListener callback)
        {
        }

        public void AddModifier(string statName, StatModifier mod, object source)
        {
        }

        public void AddStat(string statName, float baseValue, float min = 0, float max = float.MaxValue)
        {
        }

        public void CalculateStats()
        {
        }

        public void ClearAllModifiers()
        {
        }

        public void ClearAllStat()
        {
        }

        public void Copy(IStatGroup refer, float percentage = 1)
        {
        }

        public Dictionary<string, Stat> GetAllStat()
        {
            return null;
        }

        public float GetBaseValue(string statName, float defaultValue = 0)
        {
            return 0;
        }

        public float GetLastValue(string statName, float defaultValue = 0)
        {
            return 0;
        }

        public float GetMaxConstraint(string statName)
        {
            return 0;
        }

        public float GetMinConstraint(string statName)
        {
            return 0;
        }

        public IEnumerable<StatModifier> GetModifiers(string statName)
        {
            return null;
        }

        public Stat GetStat(string statName)
        {
            return null;
        }

        public float GetValue(string statName, float defaultValue = 0)
        {
            return 0;
        }

        public bool HasModifier(string statName, StatModifier modifier)
        {
            return false;
        }

        public bool HasStat(string statName)
        {
            return false;
        }

        public void RemoveAllStats()
        {
        }

        public void RemoveListener(string statName, StatChangeListener callback)
        {
        }

        public void RemoveModifier(string statName, StatModifier mod)
        {
        }

        public void RemoveModifiersFromSource(object source)
        {
        }

        public void RemoveStat(string statName)
        {
        }

        public void ReplaceAllStatBySource(IStatGroup refer, params object[] source)
        {
        }

        public IStatGroup SetBaseValue(string statName, float value, bool callUpdater = true)
        {
            return this;
        }
        public List<StatModifier> GetModifiersFromSource(string statName, object source)
        {
            var stat = GetStat(statName);
            if (stat == null) return new List<StatModifier>();
            return stat.AttributeModifiers;
        }
        public IStatGroup SetMaxValue(string statName, float max)
        {
            return this;
        }

        public IStatGroup SetMinValue(string statName, float min)
        {
            return this;
        }
    }
}