using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [Serializable]
    public class GroupModifier
    {
        [SerializeField] private Dictionary<string, StatModifier> modifiers;

        public GroupModifier()
        {
            modifiers = new Dictionary<string, StatModifier>();
        }

        ~GroupModifier()
        {
            modifiers.Clear();
        }

        public void RemoveAllMod()
        {
            modifiers.Clear();
        }
        public bool HasStat(string key)
        {
            return modifiers.ContainsKey(key);
        }
        public void AddModifier(string key, StatModifier mod)
        {
            if (!modifiers.ContainsKey(key))
            {
                modifiers.Add(key, mod);
            }
        }

        public StatModifier GetModifier(string key)
        {
            return modifiers[key];
        }

        public Dictionary<string, StatModifier> GetAllModifier()
        {
            return modifiers;
        }
    }
}