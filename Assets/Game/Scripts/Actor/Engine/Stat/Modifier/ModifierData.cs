using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Engine;

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
}