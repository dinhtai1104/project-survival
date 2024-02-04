using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGroupAffixText : MonoBehaviour
{
    private List<UIAffixText> _affixText = new List<UIAffixText>();

    public void Clear()
    {
        foreach (var aff in _affixText)
        {
            PoolManager.Instance.Despawn(aff.gameObject);
        }
        _affixText.Clear();
    }
    public void AddAffixText(UIAffixText affix)
    {
        _affixText.Add(affix);
    }
}