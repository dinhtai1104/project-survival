using Assets.Game.Scripts.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryHeroItem : MonoBehaviour
{
    public delegate void HeroClicked(EHero hero);
    public HeroClicked heroClicked;

    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private UIStarHolder starHeroHolder;
    [SerializeField] private Image elementHeroTypeImg;
    [SerializeField] private GameObject isUseHeroGO;
    [SerializeField] private GameObject lockedObject;
    [SerializeField] private Image HeroAvatarImg;
    [SerializeField] private GameObject notiObject;
    [SerializeField] private List<NotifyCondition<EHero>> notiCondition;

    private HeroSave saveEntity;
    private HeroEntity heroEntity;
    private UserSave userSave;

    public EHero HeroType => heroEntity.TypeHero;
    private bool isCurrentPick;
    public void Set(HeroEntity entity, bool isCurrent)
    {
        this.heroEntity = entity;
        this.userSave = DataManager.Save.User;
        saveEntity = userSave.HeroSaves[heroEntity.TypeHero];
        isCurrentPick = isCurrent;
        ShowInformation(isCurrent);
        HeroAvatarImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Avatar, entity.TypeHero.ToString());
        var rs = false;
        foreach (var condi in notiCondition)
        {
            var r = condi.Validate(entity.TypeHero);
            if (r == true)
            {
                rs = true;
                break;
            }
        }
        notiObject.SetActive(rs);
    }

    private void ShowInformation(bool isCurrent)
    {
        isUseHeroGO.SetActive(isCurrent);
        starHeroHolder.SetStar(ConstantValue.MaxStarHeroUpgrade);
        elementHeroTypeImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{heroEntity.StoneResource}");
        lockedObject.SetActive(!saveEntity.IsUnlocked);

        if (!saveEntity.IsUnlocked)
        {
            ShowLocked();
        }
        else
        {
            ShowUnlocked();
        }
        var rs = false;
        foreach (var condi in notiCondition)
        {
            var r = condi.Validate(saveEntity.Type);
            if (r == true)
            {
                rs = true;
                break;
            }
        }
        notiObject.SetActive(rs);
    }

    private void ShowUnlocked()
    {
        levelTxt.text = $"Lv {saveEntity.Level}";
        starHeroHolder.SetStarsActive(saveEntity.Star);
    }

    private void ShowLocked()
    {
        levelTxt.text = "";
        starHeroHolder.SetStarsActive(0);
    }

    public void OnClicked()
    {
        heroClicked?.Invoke(heroEntity.TypeHero);
        Messenger.Broadcast(EventKey.SelectHeroInventory, heroEntity.TypeHero);
    }

    private void OnEnable()
    {
     //   Messenger.AddListener<EHero>(EventKey.UpdateHeroItemUI, OnUpdateHero);
    }
    private void OnDisable()
    {
        isCurrentPick = false;
   //     Messenger.RemoveListener<EHero>(EventKey.UpdateHeroItemUI, OnUpdateHero);
        heroClicked = null;
    }

    //private void OnUpdateHero(EHero eHero)
    //{
    //    if (eHero != saveEntity.Type) 
    //    {
    //        ShowInformation(false);
    //        return;
    //    }
    //    ShowInformation(true);
    //}

    public void Pick()
    {
        ShowInformation(true);
    }
    public void UnPick()
    {
        ShowInformation(false);
    }
}