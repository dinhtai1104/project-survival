using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GroupModifier
{
    [SerializeField] private Dictionary<StatKey, StatModifier> modifiers;

    public GroupModifier()
    {
        modifiers = new Dictionary<StatKey, StatModifier>();
    }

    ~GroupModifier()
    {
        modifiers.Clear();
    }

    public void RemoveAllMod()
    {
        modifiers.Clear();
    }
    public bool HasStat(StatKey key)
    {
        return modifiers.ContainsKey(key);
    }
    public void AddModifier(StatKey key, StatModifier mod)
    {
        if (!modifiers.ContainsKey(key))
        {
            modifiers.Add(key, mod);
        }
    }

    public StatModifier GetModifier(StatKey key)
    {
        return modifiers[key];
    }

    public Dictionary<StatKey,StatModifier> GetAllModifier()
    {
        return this.modifiers;
    }
}