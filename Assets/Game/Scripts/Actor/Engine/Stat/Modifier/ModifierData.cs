using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Engine;
using System;

[System.Serializable]
public class ModifierData
{
    [SerializeField]
    [ValueDropdown("@DropdownKey.Stat")]
    private string m_AttributeName;

    [SerializeField, InlineProperty, HideLabel]
    private StatModifier m_Modifier;

    public string AttributeName => m_AttributeName;
    public StatModifier Modifier => m_Modifier;

    public ModifierData()
    {
    }

    public ModifierData(string attributeName, StatModifier modifier)
    {
        m_AttributeName = attributeName;
        m_Modifier = modifier;
    }

    public ModifierData(string param)
    {
        var split = param.Trim().Split(';');
        m_AttributeName = split[0];

        Enum.TryParse(split[1], out EStatMod mod);
        m_Modifier = new StatModifier(mod, float.Parse(split[2]));
    }
}