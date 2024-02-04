using System;
using TMPro;
using UnityEngine;

public class UIDebugStatModifierItem : MonoBehaviour
{
    public TextMeshProUGUI value;
    public TextMeshProUGUI type;
    public TextMeshProUGUI source;
    public void Setup(StatModifier mod)
    {
        value.text= "Value: " + mod.Value.ToString();
        type.text= "Type: " + mod.Type.ToString();
        source.text= "Source: " + mod.Source.ToString();
    }
}