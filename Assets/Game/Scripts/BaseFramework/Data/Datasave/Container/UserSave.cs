using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class UserSave : BaseDatasave
{
    public string Id;
    public string PlayerName;
    /// <summary>
    /// Exp to upgrade level
    /// </summary>
    public long Experience;
    private int level = 2;
    public int Level => level;
    public bool CanShowHeroSale = false;
    public int NumberPlayGame = 0;
    public int NumberBackMenu = 0;
    public int SessionGame = 0;
    public double OnlineTime; // Second;

    public int IAP_Count = 0;
    public int IAA_Count = 0;

    #region Dungeon

    #endregion

    #region HERO
    /// <summary>
    /// Hero Picked Now
    /// </summary>
    public EHero Hero;
    /// <summary>
    /// Get Hero Current, if trying new hero return TryHero else return Hero user
    /// </summary>
    public EHero HeroCurrent
    {
        get
        {
            // If TryHeroCurrent Exist
            if (TryHeroCurrent == EHero.None)
            {
                return Hero;
            }
            return TryHeroCurrent;
        }
    }
    /// <summary>
    /// Hero Try If exist
    /// </summary>
    public EHero TryHeroCurrent = EHero.None;

    //public List<HeroSave> HeroSaves = new List<HeroSave>();
    [ShowInInspector]
    public Dictionary<EHero, HeroSave> HeroSaves = new Dictionary<EHero, HeroSave>();
    #endregion

    public List<IAPSave> iapBought = new List<IAPSave>();

    public bool IsFirstDie = false;
    public bool CheckFirstDie = false;
    public UserSave(string key) : base(key)
    {
        TryHeroCurrent = EHero.None;
        Hero = EHero.NormalHero;
        HeroSaves.Add(EHero.NormalHero, new HeroSave { Star = 0, Type = EHero.NormalHero, IsUnlocked = true });
        PlayerName = "Player"+UnityEngine.Random.Range(1,9999999);
        OnlineTime = 0;

        var allHeros = (EHero[])Enum.GetValues(typeof(EHero));
        foreach (var hero in allHeros)
        {
            if (hero == EHero.None || HeroSaves.ContainsKey(hero)) continue;
            HeroSaves.Add(hero, new HeroSave { Star = 0, Type = hero, IsUnlocked = false });
        }
    }

    public UserSave()
    {

    }

    public void BuyIAP(string product)
    {
        iapBought.Add(IAPSave.Buy(product));
        IAP_Count++;
        Save();
    }
    public void WatchIAA()
    {
        IAA_Count++;
        Save();
    }

    public void SetId(string id)
    {
        this.Id = id;
        Save();
    }
    public void Play()
    {
        NumberPlayGame++;
        Save();
    }

    public void PlaySession()
    {
        SessionGame++;
        Save();
    }

    public override void Fix()
    {
        foreach (var hero in HeroSaves)
        {
            hero.Value.Fix();
        }
        CanShowHeroSale = false;


        var lastZoneSave = DataManager.Save.DungeonSession;
        var lastZoneEventSave = DataManager.Save.DungeonEventSession;
        if (!lastZoneSave.IsPlaying)
        {
            if (!lastZoneEventSave.IsPlaying)
            {
                SetTryHero(EHero.None);
            }
        }
    }
    public void UpgradeStarHero(EHero eHero)
    {
        HeroSaves[eHero].UpgradeStar();
        Save();
    }
    public void UpgradeLevelHero(EHero eHero)
    {
        HeroSaves[eHero].UpgradeLevel();
        Save();

        int max = HeroSaves.Max(t => t.Value.Level);
        Architecture.Get<AchievementService>().SetProgress(EAchievement.HeroLevel, max);
    }

    public void SetTryHero(EHero hero)
    {
        TryHeroCurrent = hero;
        Save();
    }

    public void SetPickHero(EHero hero)
    {
        this.Hero = hero;
        Save();
    }

    public HeroSave GetHeroCurrent()
    {
        if (TryHeroCurrent != EHero.None)
        {
            return new HeroSave { Star = 0, Type = TryHeroCurrent };
        }
        return HeroSaves[HeroCurrent];
        throw new Exception("Not found this hero: " + HeroCurrent);
    }

    [Button]
    public void AddExp(long exp)
    {
        this.Experience += exp;
        GameSceneManager.Instance.PlayerData.ExpHandler.Add(exp);
        Save();
    }

    public bool IsUnlockHero(EHero key)
    {
        return HeroSaves[key].IsUnlocked;
    }

    public HeroSave GetHero(EHero hero)
    {
        if (HeroSaves.ContainsKey(hero))
        {
            return HeroSaves[hero];
        }
        return null;
    }

    [Button]
    public void UnlockHero(EHero key)
    {
        HeroSaves[key].Unlocked();
        Save();

        switch (key)
        {
            case EHero.PoisonHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroPoison);
                break;
            case EHero.FrozenHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroFrozen);
                break;
            case EHero.NinjaHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroNinja);
                break;
            case EHero.JumpHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroJump);
                break;
            case EHero.RocketHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroRocket);
                break;
            case EHero.ShinigamiHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroShinigami);
                break;
            case EHero.CowboyHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroCowboy);
                break;
            case EHero.AngelHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroAngel);
                break;
            case EHero.EvilHero:
                 Architecture.Get<AchievementService>().IncreaseProgress(EAchievement.OwnHeroEvil);
                break;
        }
    }
}