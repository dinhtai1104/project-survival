using Assets.Game.Scripts.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryHeroInforView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameHeroTxt;
    [SerializeField] private Image elementHeroImg;
    [SerializeField] private UIStarHolder starHeroHolder;
    [SerializeField] private TextMeshProUGUI levelHeroTxt;

    private void OnEnable()
    {
        Messenger.AddListener<EHero>(EventKey.UpdateHeroItemUI, OnPickHero);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<EHero>(EventKey.UpdateHeroItemUI, OnPickHero);
    }

    public void OnPickHero(EHero hero)
    {
        starHeroHolder.gameObject.SetActive(true);
        starHeroHolder.SetStar(ConstantValue.MaxStarHeroUpgrade);

        var userSave = DataManager.Save.User;
        var entity = DataManager.Base.Hero.GetHero(hero);
        if (userSave.IsUnlockHero(hero))
        {
            var heroSave = userSave.GetHero(hero);
            starHeroHolder.SetStarsActive(heroSave.Star);
            levelHeroTxt.text = $"Lv. {heroSave.Level}";
        }
        else
        {
            starHeroHolder.SetStarsActive(0);
            starHeroHolder.gameObject.SetActive(false);
            levelHeroTxt.text = $"";
        }
        nameHeroTxt.text = I2Localize.GetLocalize($"Hero_Name/{entity.TypeHero}");
        elementHeroImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{entity.StoneResource}");
    }
}