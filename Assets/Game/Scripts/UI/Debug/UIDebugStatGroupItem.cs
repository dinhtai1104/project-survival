using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class UIDebugStatGroupItem : MonoBehaviour
{
    public UIDebugStatModifierItem prefabModifer;
    public TextMeshProUGUI statKeyName;
    public TextMeshProUGUI baseValueTxt;
    public TextMeshProUGUI valueTxt;
    public Transform holderModifier;

    public Stat stat;
    public StatKey statKey;
    public void Setup(Stat stat, StatKey statkey)
    {
        this.stat = stat;
        this.statKey = statkey;
        statKeyName.text = statKey.ToString();
        baseValueTxt.text = "Base: " + stat.BaseValue.ToString();
        valueTxt.text = "Value: " + stat.Value.ToString();

        foreach (var mod in stat.AttributeModifiers)
        {
            var modif = Instantiate(prefabModifer, holderModifier);
            modif.Setup(mod);
            modif.gameObject.SetActive(true);
        }
    }
}