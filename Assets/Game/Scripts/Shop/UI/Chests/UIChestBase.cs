using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using com.mec;
using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using GameUtility;
using GoogleMobileAds.Api;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIChestBase : MonoBehaviour
{
    public EChest chestType;
    public int NumberOpen = 1;
    [SerializeField] protected TextMeshProUGUI nameChest;
    [SerializeField] protected TextMeshProUGUI descriptionChest;

    protected ChestEntity entity;
    protected ChestBaseSave save;
    protected ResourcesSave resource;
    protected UserSave userSave;
    protected InventorySave inventory;
    [SerializeField] private TextMeshProUGUI m_TimeFreeTxt;
    [SerializeField] private TextMeshProUGUI m_AdFreeLeft;

    [Header("Button")]
    [SerializeField] private Button m_ButtonAds;
    [SerializeField] private Button m_ButtonGem;
    [SerializeField] private Button m_ButtonKey;

    [SerializeField] private UIIconText m_IconGem;
    [SerializeField] private UIIconText m_IconKey;

    private void OnEnable()
    {
        Messenger.AddListener<EResource>(EventKey.UpdateResource, OnUpdateCurrency);
    }

    private void OnUpdateCurrency(EResource type)
    {
        Setup();
    }

    public virtual void Init()
    {
        this.resource = DataManager.Save.Resources;
        this.entity = DataManager.Base.Chest.Dictionary[chestType];
        this.save = DataManager.Save.Chest.Saves[chestType];
        this.userSave = DataManager.Save.User;
        this.inventory = DataManager.Save.Inventory;
        Setup();
    }

    protected virtual void Setup()
    {
        m_TimeFreeTxt.text = "";
        m_ButtonAds.gameObject.SetActive(false);
        m_ButtonGem.gameObject.SetActive(false);
        m_ButtonKey.gameObject.SetActive(false);
        
        m_AdFreeLeft.text = $"{save.AdLimit}";
        if (entity.MaxRewardDay == -1)
        {
            m_AdFreeLeft.text = $"{1}";
        }
        // Not support ads open
        if (entity.TimeRewardAd == -1)
        {
            m_ButtonGem.gameObject.SetActive(true);
            m_ButtonKey.gameObject.SetActive(true);

            if (entity.CurrencyCost.Value == -1)
            {
                m_ButtonGem.gameObject.SetActive(false);
            }
            else
            {
                m_IconGem.Set(entity.CurrencyCost);
            }
            if (entity.KeyCost.Resource == EResource.None)
            {
                m_ButtonKey.gameObject.SetActive(false);
            }
            else
            {
                m_IconKey.Set(entity.KeyCost);
            }
            return;
        }
        else
        {
            m_IconGem.Set(entity.CurrencyCost);
            m_IconKey.Set(entity.KeyCost, (long)resource.GetResource(entity.KeyCost.Resource));

            var lastTime = save.TimeLastOpenAd;
            var timeCooldown = new TimeSpan(0, 0, entity.TimeRewardAd);
            var left = (UnbiasedTime.UtcNow - lastTime);

            if (resource.HasResource(entity.KeyCost))
            {
                m_ButtonKey.gameObject.SetActive(true);
            }
            else if (resource.HasResource(entity.CurrencyCost))
            {
                m_ButtonGem.gameObject.SetActive(true);
            }
            else
            {
                m_ButtonGem.gameObject.SetActive(true);
            }
            m_ButtonAds.gameObject.SetActive(true);
            

            if (entity.TimeRewardAd == -1 || entity.MaxRewardDay == -1)
            {
                // Khong dung xem ad trong ngay
                m_AdFreeLeft.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                m_AdFreeLeft.transform.parent.gameObject.SetActive(true);
            }


            if (left.TotalSeconds > entity.TimeRewardAd)
            {
                Timing.KillCoroutines(gameObject);
            }
            else
            {
                //m_ButtonAds.gameObject.SetActive(false);
                if ((save.AdLimit > 0 || entity.MaxRewardDay == -1) && entity.TimeRewardAd != -1)
                {
                    Timing.RunCoroutine(_Ticks(), gameObject);
                }
                else
                {
                    m_TimeFreeTxt.text = "";
                }
            }
            if (save.AdLimit <= 0 && entity.MaxRewardDay != -1)
            {
                //m_ButtonAds.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator<float> _Ticks()
    {
        var lastTime = save.TimeLastOpenAd;
        var timeCooldown = new TimeSpan(0, 0, entity.TimeRewardAd);
        while (true)
        {
            var left = timeCooldown - (UnbiasedTime.UtcNow - lastTime);
            m_TimeFreeTxt.SetText(I2Localize.GetLocalize("Common/Title_FreeIn") + left.ConvertTimeToString());
            if (left.TotalSeconds <= 0) break;
            yield return Timing.WaitForSeconds(1);
        }
        Setup();
    }

    protected virtual void OnDisable()
    {
        Messenger.RemoveListener<EResource>(EventKey.UpdateResource, OnUpdateCurrency);
        Timing.KillCoroutines(gameObject);
    }


    public virtual void OpenViaAdsOnClicked()
    {
        var lastTime = save.TimeLastOpenAd;
        var timeCooldown = new TimeSpan(0, 0, entity.TimeRewardAd);
        var left = timeCooldown - (UnbiasedTime.UtcNow - lastTime);

        if (save.AdLimit > 0 || entity.MaxRewardDay == -1)
        {
            lastTimeClick = Time.time;
            if (left.TotalSeconds > 0)
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.CooldownTimeNotEnd")).ContinueWith(t =>
                {
                    t.onClosed += () => {
                        switch (entity.KeyCost.Resource)
                        {
                            case EResource.SilverKey:
                                MenuGameScene.Instance.EnQueue(EFlashSale.SilverKey);
                                break;
                            case EResource.GoldenKey:
                                MenuGameScene.Instance.EnQueue(EFlashSale.GoldenKey);
                                break;
                            case EResource.HeroKey:
                                MenuGameScene.Instance.EnQueue(EFlashSale.HeroKey);
                                break;
                        }
                    };
                }).Forget();

                return;
            }
           
            FirebaseAnalysticController.Tracker.NewEvent(GetPlacement(chestType)).Track();

            Architecture.Get<AdService>().ShowRewardedAd(GetPlacement(chestType), res =>
            {
                if (res)
                {
                    save.OpenChest();

                    save.AdLimit--;
                    Open();
                    save.SetLastTimeOpenAd(UnbiasedTime.UtcNow);
                    Setup();
                }
                else
                {
                    PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd")).ContinueWith(t =>
                    {
                        t.onClosed += () => {
                            switch (entity.KeyCost.Resource)
                            {
                                case EResource.SilverKey:
                                    MenuGameScene.Instance.EnQueue(EFlashSale.SilverKey);
                                    break;
                                case EResource.GoldenKey:
                                    MenuGameScene.Instance.EnQueue(EFlashSale.GoldenKey);
                                    break;
                                case EResource.HeroKey:
                                    MenuGameScene.Instance.EnQueue(EFlashSale.HeroKey);
                                    break;
                            }
                        };
                    }).Forget();
                }
            }, placement: GetPlacement(chestType));

            string GetPlacement(EChest type)
            {
                switch (type)
                {
                    case EChest.Silver:
                        return AD.AdPlacementKey.SILVER_CHEST;
                    case EChest.Golden:
                        return AD.AdPlacementKey.GOLDEN_CHEST;
                    case EChest.Hero:
                        return AD.AdPlacementKey.HERO_CHEST;
                    default:
                        return null;
                }
            }

        }
        else
        {
            PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.FullAdOpen")).ContinueWith(t =>
            {
                t.onClosed += () => {
                    switch (entity.KeyCost.Resource)
                    {
                        case EResource.SilverKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.SilverKey);
                            break;
                        case EResource.GoldenKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.GoldenKey);
                            break;
                        case EResource.HeroKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.HeroKey);
                            break;
                    }
                };
            }).Forget();
        }

    }
    public virtual void OpenViaGemOnClicked()
    {
        if (resource.HasResource(entity.CurrencyCost))
        {
            if (Time.time - lastTimeClick >= 0.5f)
            {
                save.OpenChest();
                lastTimeClick = Time.time;
                Open();
                Setup();
                resource.DecreaseResource(entity.CurrencyCost);

                // Track
                FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
                    .AddStringParam("item_category", entity.CurrencyCost.GetAllData()[0].Type.ToString())
                    .AddStringParam("item_id", entity.CurrencyCost.Resource.ToString())
                    .AddStringParam("source", "shop")
                    .AddStringParam("source_id", "{0}_chest".AddParams(chestType.ToString().ToLower()))
                    .AddDoubleParam("value", entity.CurrencyCost.Value)
                    .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(entity.CurrencyCost.Resource))
                    .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(entity.CurrencyCost.Resource))
                    .Track();
            }
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, entity.CurrencyCost.Resource.GetLocalize())).ContinueWith(t =>
            {
                t.onClosed += () => MenuGameScene.Instance.EnQueue(EFlashSale.Gem);
            }).Forget();
        }
    }

    private void OpenChestMission()
    {
        for (int i = 0; i < NumberOpen; i++)
        {
            switch (chestType)
            {
                case EChest.Silver:
                    Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.OpenChestSilver);
                    Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OpenChestSilver);
                    break;
                case EChest.Golden:
                case EChest.Golden10:
                    Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.OpenChestGolden);
                    Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OpenChestGolden);
                    break;
                case EChest.Hero:
                case EChest.Hero10:
                    Architecture.Get<QuestService>().IncreaseProgress(EMissionDaily.OpenChestHero);
                    Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OpenChestHero);
                    break;
            }
        }
    }

    private float lastTimeClick = 0;
    public virtual void OpenViaKeyOnClicked()
    {
        if (resource.HasResource(entity.KeyCost))
        {
            if (Time.time - lastTimeClick >= 0.5f)
            {
                save.OpenChest();
                lastTimeClick = Time.time;
                Open();
                resource.DecreaseResource(entity.KeyCost);
                Setup();

            }
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, entity.KeyCost.Resource.GetLocalize())).ContinueWith(t =>
            {
                t.onClosed += () =>
                {
                    switch (entity.KeyCost.Resource)
                    {
                        case EResource.SilverKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.SilverKey);
                            break;
                        case EResource.GoldenKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.GoldenKey);
                            break;
                        case EResource.HeroKey:
                            MenuGameScene.Instance.EnQueue(EFlashSale.HeroKey);
                            break;
                    }
                };
            }).Forget();

           
        }
    }

    protected virtual void Open()
    {
        OpenChestMission();
    }

    protected virtual List<LootParams> GetReward()
    {
        var listRewards = new List<LootParams>();
        var keys = entity.Rewards.Keys;
        foreach (var rewardType in keys)
        {
            var weights = entity.Rewards[rewardType];
            var random = weights.RandomWeighted();
            listRewards.Add(weights[random].Clone());
        }
        return listRewards;
    }
}