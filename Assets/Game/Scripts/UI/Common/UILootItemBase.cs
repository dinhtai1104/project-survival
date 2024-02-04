using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UILootItemBase : UIGeneralBaseIcon
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI amountTxt;
    public override Sprite Sprite => iconImg.sprite;
    protected ILootData data;
    public ILootData Data => data;

    protected virtual void OnEnable()
    {
    }
    protected virtual void OnValidate()
    {
    }
    public void SetSprite(Sprite icon)
    {
        iconImg.sprite = icon;
    }

    public override void SetSizeImage(float size = 1)
    {
        base.SetSizeImage(size);
        iconImg.rectTransform.localScale = Vector3.one * size;
    }
    public void SetAmount(long amount)
    {
        this.amountTxt.text = amount.TruncateValue();
    }
    public void SetText(string text)
    {
        this.amountTxt.text = text;
    }
}