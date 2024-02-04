using Assets.Game.Scripts.Utilities;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBuffItemBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected TextMeshProUGUI buffNameTxt;
    [SerializeField] protected Image buffIconImg;
    [SerializeField] protected Image buffOutlineImg;

    protected int Level;
    protected BuffEntity buffEntity;
    protected BuffSave buffSave;

    public EPickBuffType buffType;

    protected virtual void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
    public virtual void SetEntity(BuffEntity entity)
    {
        this.buffEntity = entity;
    }
    public BuffEntity GetEntity() => buffEntity;
    public virtual void PrepareAnimation()
    {
    }
    public virtual void SetInfor()
    {
        var iconBuff = ResourcesLoader.Instance.GetSprite(AtlasName.Buff, buffEntity.Icon);
        buffIconImg.sprite = iconBuff;
        buffNameTxt.text = I2Localize.GetLocalize($"Buff_Name/{buffEntity.Type}");

        buffOutlineImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Common, $"buff_border_{(int)buffEntity.Classify + 1}");
    }

    public void SetStarActive(int value)
    {
        Level = value;
    }
    public int GetStar() 
    {
        return Level;
    }
    public virtual void SetDescription()
    {
        
    }
}