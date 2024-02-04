using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// StatGroup for any object has more than one stat, it can control stats of object
/// </summary>
/// 
[System.Serializable]
public class StatGroup : IStatGroup
{
    private readonly Dictionary<StatKey, Stat> _attrDict;
    public IEnumerable<StatKey> StatNames => _attrDict.Keys;

    [ShowInInspector]
    public Dictionary<StatKey, Stat> AttributesDirectDict => _attrDict;
    public StatGroup()
    {
        _attrDict = new Dictionary<StatKey, Stat>();
    }
    public IStatGroup SetMinValue(StatKey name, float min)
    {
        if (_attrDict.ContainsKey(name)) _attrDict[name].SetConstraintMin(min);
        return this;
    }
    public IStatGroup SetMaxValue(StatKey name, float max)
    {
        if (_attrDict.ContainsKey(name)) _attrDict[name].SetConstraintMax(max);
        return this;
    }

    public void AddStat(StatKey name, float baseValue, float min = float.MinValue, float max = float.MaxValue)
    {
        if (_attrDict.ContainsKey(name))
        {
            Debug.LogError("Duplicated attribute " + name);
        }
        else
        {
            Stat stat = new Stat { BaseValue = baseValue };
            stat.SetConstraintMin(min);
            stat.SetConstraintMax(max);

            _attrDict.Add(name, stat);
        }
    }
    public Stat CreateStat(StatKey name, float baseValue, float min = float.MinValue, float max = float.MaxValue)
    {
        if (_attrDict.ContainsKey(name)) return null;
        Stat stat = new Stat { BaseValue = baseValue };
        stat.SetConstraintMin(min);
        stat.SetConstraintMax(max);

        _attrDict.Add(name, stat);
        return stat;
    }
    public bool HasStat(StatKey name)
    {
        return _attrDict.ContainsKey(name);
    }

    public bool HasModifier(StatKey statName, StatModifier modifier)
    {
        if (!HasStat(statName)) return false;
        return _attrDict[statName].HasModifier(modifier);
    }
    public void CalculateStats()
    {
        foreach (var stat in _attrDict.Values)
        {
            stat.RecalculateValue();
        }
    }

    public void AddListener(StatKey statName, Action<float> callback)
    {
        if (!_attrDict.ContainsKey(statName)) return;
        Stat stat = _attrDict[statName];
        stat.AddListener(callback);
    }

    public void RemoveListener(StatKey statName, Action<float> callback)
    {
        if (!_attrDict.ContainsKey(statName)) return;
        _attrDict[statName].RemoveListener(callback);
    }

    public IStatGroup SetBaseValue(StatKey statName, float value, bool callUpdater = true)
    {
        if (_attrDict != null && _attrDict.ContainsKey(statName))
        {
            _attrDict[statName].BaseValue = value;
        }
        else
        {
            //Debug.Log(gameObject.name + " " + name + " is not in attribute dictionary");
        }

        return this;
    }


    public float GetBaseValue(StatKey name, float defaultValue = 0f)
    {
        if (_attrDict == null) return 0f;

        if (_attrDict.ContainsKey(name)) return _attrDict[name].BaseValue;

        //            Debug.LogError(name + " is not in attribute dictionary");
        return defaultValue;
    }

    public float GetValue(StatKey name, float defaultValue = 0f)
    {
        if (_attrDict == null) return defaultValue;

        return _attrDict.ContainsKey(name) ? _attrDict[name].Value : defaultValue;
    }

    public float GetLastValue(StatKey statName, float defaultValue = 0f)
    {
        if (_attrDict == null) return defaultValue;

        return _attrDict.ContainsKey(statName) ? _attrDict[statName].LastValue : defaultValue;
    }

    public float GetMinConstraint(StatKey name)
    {
        if (_attrDict == null) return 0f;

        return _attrDict.ContainsKey(name) ? _attrDict[name].ConstraintMin : 0f;
    }

    public float GetMaxConstraint(StatKey name)
    {
        if (_attrDict == null) return float.MaxValue;

        return _attrDict.ContainsKey(name) ? _attrDict[name].ConstraintMax : float.MaxValue;
    }


    public void AddModifier(StatKey statName, StatModifier mod, object source)
    {
        if (!_attrDict.ContainsKey(statName))
        {
            Debug.Log(" Attribute key does not exist: " + statName);
        }
        else
        {
            Stat attr = _attrDict[statName];
            mod.Source = source;
            attr.AddModifier(mod);
        }
    }

    public void RemoveModifier(StatKey name, StatModifier mod)
    {
        if (_attrDict.ContainsKey(name))
        {
            Stat attr = _attrDict[name];
            attr.RemoveModifier(mod);
        }
    }

    public void RemoveModifiersFromSource(object source)
    {
        foreach (var key in _attrDict.Keys)
        {
            _attrDict[key].RemoveAllModifiersFromSource(source);
        }
    }

    public void ClearAllModifiers()
    {
        foreach (var stat in _attrDict.Values)
        {
            stat.ClearModifiers();
        }
    }

    public Stat GetStat(StatKey statName)
    {
        if (_attrDict.ContainsKey(statName))
        {
            return _attrDict[statName];
        }
        return null;
        //throw new Exception("Not contains: " + statName);
    }

    public void RemoveStat(StatKey statName)
    {
        if (_attrDict.ContainsKey(statName))
        {
            _attrDict.Remove(statName);
        }
    }

    public void RemoveAllStats()
    {
        _attrDict.Clear();
    }

    public Dictionary<StatKey, Stat> GetAllStat()
    {
        return _attrDict;
    }

    public void Copy(IStatGroup refer, float percentage = 1)
    {
        foreach (var attributeName in refer.StatNames)
        {
            if (HasStat(attributeName))
            {
                SetBaseValue(attributeName, refer.GetBaseValue(attributeName) * percentage);
            }
            else
            {
                AddStat(attributeName, refer.GetBaseValue(attributeName), refer.GetMinConstraint(attributeName), refer.GetMaxConstraint(attributeName));
            }

            foreach (var mod in refer.GetModifiers(attributeName))
            {
                AddModifier(attributeName, mod, mod.Source);
            }
        }
    }

    public IEnumerable<StatModifier> GetModifiers(StatKey statName)
    {
        if (_attrDict == null || !_attrDict.ContainsKey(statName)) return Enumerable.Empty<StatModifier>();

        return _attrDict[statName].AttributeModifiers;
    }

    public void ReplaceAllStatBySource(IStatGroup refer, object[] source)
    {
        foreach (var attributeName in refer.StatNames)
        {
            if (HasStat(attributeName))
            {
                SetBaseValue(attributeName, refer.GetBaseValue(attributeName));
            }
            else
            {
                AddStat(attributeName, refer.GetBaseValue(attributeName), refer.GetMinConstraint(attributeName), refer.GetMaxConstraint(attributeName));
            }
        }
        foreach (var soucee in source)
        {
            RemoveModifiersFromSource(soucee);
            foreach (var attributeName in refer.StatNames)
            {
                if (HasStat(attributeName))
                {
                    foreach (var mod in refer.GetModifiers(attributeName))
                    {
                        if (mod.Source.Equals(soucee))
                        {
                            AddModifier(attributeName, mod, soucee);
                        }
                    }

                    GetStat(attributeName).InvokeListeners();
                }
            }
        }
    }
}