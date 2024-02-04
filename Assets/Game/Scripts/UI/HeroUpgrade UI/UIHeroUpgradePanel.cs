using Assets.Game.Scripts.Utilities;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UI;
using UiParticles;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroUpgradePanel : UI.Panel
{
    [SerializeField] private RectTransform heroParent;
    private UIActor heroView;
    [SerializeField] private UIInventoryHeroesView allHeroesView;
    [SerializeField] private UIInventoryHeroInforView heroInforView;
    [SerializeField] private UIHeroUpgradeView heroUpgradeView;
    private UserSave userSave;
    private ResourcesSave resourcesSave;

    private IStatGroup statData;
    [SerializeField] private UIStatItem dmgStat;
    [SerializeField] private UIStatItem hpStat;
    private PlayerData playerData;
    private EHero heroCurrent = EHero.None;
    private ResourceData fragmentUnlockHero;

    [Header("Button")]
    [SerializeField] private GameObject buttonEquiped;
    [SerializeField] private GameObject buttonEquipable;
    [SerializeField] private GameObject buttonUnlocked;
    [SerializeField] private Image heroUnlock_FragmentImg;
    [SerializeField] private TextMeshProUGUI heroUnlock_FragmentAmount;

    [Header("Hero Infor")]
    [SerializeField] private TextMeshProUGUI namePassiveSkillTxt;
    [SerializeField] private TextMeshProUGUI descriptionPassiveSkillTxt;

    [SerializeField] private UIParticle effect_Hero_Level_Up;
    [SerializeField] private UIParticle effect_Hero_Star_Up;

    [SerializeField] private UITweenBase tweenUpgradePanel;
    private void OnEnable()
    {
        Messenger.AddListener(EventKey.HeroUpgradeStarSuccess, OnHeroUpgradeStarSuccess);
        Messenger.AddListener(EventKey.HeroUpgradeLevelSuccess, OnHeroUpgradeLevelSuccess);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.HeroUpgradeStarSuccess, OnHeroUpgradeStarSuccess);
        Messenger.RemoveListener(EventKey.HeroUpgradeLevelSuccess, OnHeroUpgradeLevelSuccess);
    }

    private void OnHeroUpgradeLevelSuccess()
    {
        effect_Hero_Level_Up.Play();
    }

    private void OnHeroUpgradeStarSuccess()
    {
        effect_Hero_Star_Up.Play();
    }

    public override void PostInit()
    {
        playerData = GameSceneManager.Instance.PlayerData;
        userSave = DataManager.Save.User;
        resourcesSave = DataManager.Save.Resources;
    }

    public void RefreshStat()
    {
        dmgStat.SetData(StatKey.Dmg, statData.GetValue(StatKey.Dmg));
        hpStat.SetData(StatKey.Hp, statData.GetValue(StatKey.Hp));

        RefreshUI();
        Messenger.Broadcast(EventKey.UpdateHeroItemUI, heroCurrent);
    }

    private void RefreshUI()
    {
        heroUpgradeView.SetHero(this, heroCurrent);
    }

    public override void Show()
    {
        base.Show();
        allHeroesView.Show(HeroOnClicked);
        SetHero(userSave.Hero);
        this.statData = playerData.HeroDatas[userSave.Hero].heroStat;
        RefreshStat();
    }

    private void HeroOnClicked(EHero hero)
    {
        SetHero(hero);
        this.statData = playerData.HeroDatas[hero].heroStat;
        RefreshStat();
        tweenUpgradePanel.Init();
        tweenUpgradePanel.Show();
    }

    private EHero lastHeroPick = EHero.None;

    public void SetHero(EHero hero)
    {
        this.statData = playerData.HeroDatas[hero].heroStat;
        if (lastHeroPick != hero)
        {
            if (heroView != null)
            {
                PoolManager.Instance.Despawn(heroView.gameObject);
            }
            var ePath = string.Format(AddressableName.UIHero, hero);

            heroView = ResourcesLoader.Instance.Get<UIActor>(ePath, heroParent);
            (heroView as UIPlayerActor)?.PlayVFXAppear();
        }
        lastHeroPick = hero;
        SetPassiveInfor(hero);

        fragmentUnlockHero = null;
        heroCurrent = hero;
        heroInforView.OnPickHero(hero);
        heroUpgradeView.SetHero(this, hero);
        allHeroesView.Pick(hero);


        buttonEquipable.SetActive(false);
        buttonEquiped.SetActive(false);
        buttonUnlocked.SetActive(false);

        // If hero is unlocked and not use
        if (userSave.IsUnlockHero(hero) && userSave.Hero != hero)
        {
            buttonEquipable.SetActive(true);
            return;
        }
        // If hero is unlocked and using
        if (userSave.IsUnlockHero(hero) && userSave.Hero == hero)
        {
            buttonEquiped.SetActive(true);
            return;
        }
        // If hero is not unlocked
        buttonUnlocked.SetActive(true);

        var heroEntity = DataManager.Base.Hero.GetHero(hero);
        var fragmentHero = heroEntity.HeroResource;
        heroUnlock_FragmentImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{hero}");

        string fragmentAmount = "{0}/<color={1}>{2}</color>";
        var require = heroEntity.NumberFragmentUnlock;
        var origin = resourcesSave.GetResource(fragmentHero);
        var color = require > origin ? "red" : "green";
        fragmentUnlockHero = new ResourceData { Resource = fragmentHero, Value = require };

        fragmentAmount = CSharpExtension.FormatRequire(origin, require);
        heroUnlock_FragmentAmount.SetText(fragmentAmount);


    }

    private void SetPassiveInfor(EHero hero)
    {
        if (hero == EHero.NormalHero)
        {
            namePassiveSkillTxt.text = "";
            descriptionPassiveSkillTxt.text = "";
        }
        else
        {
            namePassiveSkillTxt.text = I2Localize.GetLocalize("Buff_Name/" + hero + "Passive");
            descriptionPassiveSkillTxt.text = I2Localize.GetLocalize("Buff_Description/" + hero + "Passive");
        }
    }

    public override void Close()
    {
        // Remove temp Hero Stat Data When User click to see hero stat
        var userSave = DataManager.Save.User;
        var currentHero = userSave.Hero;
        var heroEntity = DataManager.Base.Hero.GetHero(currentHero);

        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[heroEntity.TypeHero].heroStat, EStatSource.sourceHero);
        base.Close();
    }

    public void EquipHeroOnClicked()
    {
        userSave.SetPickHero(heroCurrent);

        Messenger.Broadcast(EventKey.UpdateHeroItemUI, heroCurrent);
        Messenger.Broadcast(EventKey.PickHero, heroCurrent);
        SetHero(heroCurrent);
    }

    public async void UnlockHeroOnClicked()
    {
        if (!resourcesSave.HasResource(fragmentUnlockHero))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, fragmentUnlockHero.Resource.GetLocalize())).Forget();
            return;
        }
        resourcesSave.DecreaseResource(fragmentUnlockHero);


        userSave.UnlockHero(heroCurrent);
        userSave.SetPickHero(heroCurrent);

        Messenger.Broadcast(EventKey.UpdateHeroItemUI, heroCurrent);
        Messenger.Broadcast(EventKey.PickHero, heroCurrent);

        var ui = await PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel);
        ui.Show(heroCurrent);

        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", fragmentUnlockHero.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", fragmentUnlockHero.Resource.ToString())
            .AddStringParam("source", "unlock_hero")
            .AddStringParam("source_id", heroCurrent.ToString())
            .AddDoubleParam("value", fragmentUnlockHero.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(fragmentUnlockHero.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(fragmentUnlockHero.Resource))
            .Track();

        SetHero(heroCurrent);
    }
}