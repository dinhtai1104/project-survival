using Assets.Game.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIconText : MonoBehaviour
{
    [SerializeField] private Image resourceImg;
    [SerializeField] private TextMeshProUGUI valueTxt;

    public void Set(Sprite sprite, string value)
    {
        resourceImg.sprite = sprite;
        valueTxt.text = value;
    }

    public void Set(ILootData data)
    {
        var resouces = data as ResourceData;
        if (resouces != null)
        {
            var sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, resouces.Resource.ToString());
            Set(sprite, resouces.Value.TruncateValue());
            return;
        }
        var expData = data as ExpData;
        if (expData != null)
        {
            var sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, ELootType.Exp.ToString());
            Set(sprite, expData.Exp.TruncateValue());
            return;
        }
    }


    public void Set(ResourceData data, long own)
    {
        Set(data);
        //SetAmountFormat($"{own.TruncateValue()}/{data.Value.TruncateValue()}");
        SetAmountFormat($"{CSharpExtension.FormatRequire(own, data.Value)}");
    }
    public void SetAmountFormat(string amount)
    {
        valueTxt.text = amount;
    }
}