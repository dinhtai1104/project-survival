using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public class TryHeroSaves : BaseDatasave
{
    public bool IsTried;
    [ShowInInspector]
    public Dictionary<EHero, TryHeroSave> Saves;
    public static TimeSpan CooldownTime;
    public int TryCount = 0; // Try In Day
    public int TryInAllGame = 0;
    public bool IsFreeHeroForFirstTime = false;
    public EHero HeroRecommend
    {
        get
        {
            return NextSessionNewHeroRecommend;
        }
        //get
        //{
        //    if (TryInAllGame < allHeroTriesSenerio.Count)
        //    {
        //        return allHeroTriesSenerio.Random();
        //    }
        //    //return EHero.RocketHero;
        //    var allFreeHeros = new List<EHero>();
        //    foreach (var e in Saves)
        //    {
        //        if (e.Value.CanPick)
        //        {
        //            allFreeHeros.Add(e.Key);
        //        }
        //    }
        //    if (allFreeHeros.Count > 0)
        //    {
        //        return allFreeHeros.Random();
        //    }
        //    return EHero.None;
        //}
    }

    public EHero NextSessionNewHeroRecommend = EHero.None;

    private ValueConfigSearch config = new ValueConfigSearch("[HeroTries]Tries", "2;5;6;4;2");
    private ValueConfigSearch maxTryCount = new ValueConfigSearch("[HeroTries]MaxTriesInDay", "2");
    private List<EHero> allHeroTriesSenerio = new List<EHero>();

    public TryHeroSaves()
    {
       
    }

    public void FreeFirstTime()
    {
        IsFreeHeroForFirstTime = true;
        Save();
    }
    public void RemoveTryHero(EHero hero)
    {
        Saves.Remove(hero);
        Save();
    }

    public TryHeroSaves(string key) : base(key)
    {
        IsTried = false;
        Saves = new Dictionary<EHero, TryHeroSave>();
        var allHero = (EHero[])System.Enum.GetValues(typeof(EHero));

        CooldownTime = new TimeSpan(24, 0, 0);
#if DEVELOPMENT
        CooldownTime = new TimeSpan(0, 1, 0);
#endif

        foreach (var hero in allHero)
        {
            if (hero == EHero.None) continue;
            var tryHero = new TryHeroSave(hero, UnbiasedTime.UtcNow);
            Saves.Add(hero, tryHero);
            tryHero.Save = Save;
        }
    }

    public override void Fix()
    {

        CooldownTime = new TimeSpan(24, 0, 0);
#if DEVELOPMENT
        CooldownTime = new TimeSpan(0, 1, 0);
#endif

        var userSave = DataManager.Save.User;
        foreach (var heroUnlocked in userSave.HeroSaves)
        {
            if (heroUnlocked.Value.IsUnlocked)
            {
                Saves.Remove(heroUnlocked.Key);
            }
        }
        allHeroTriesSenerio = new List<EHero>();
        var split = config.StringValue.Split(';');
        foreach (var e in split)
        {
            var i = e.TryGetInt();
            var hero = (EHero)i;
            allHeroTriesSenerio.Add(hero);
        }
    }
    public override void NextDay()
    {
        base.NextDay();
        Saves.Clear();
        IsTried = false;
        TryCount = 0;
        NextSessionNewHeroRecommend = EHero.None;
        var allHero = (EHero[])System.Enum.GetValues(typeof(EHero));
        var userSave = DataManager.Save.User;
        foreach (var hero in allHero)
        {
            
            if (hero == EHero.None) continue;
            if (userSave.IsUnlockHero(hero))
            {
                continue;
            }
            var tryHero = new TryHeroSave(hero, UnbiasedTime.UtcNow);
            Saves.Add(hero, tryHero);
            tryHero.Save = Save;
        }

        Save();
    }

    public void SetTried(EHero eHero)
    {
        if (eHero == DataManager.Save.User.Hero) return;
        IsTried = true;
        TryCount++;
        TryInAllGame++;
        DataManager.Save.User.SetTryHero(eHero);
        if (Saves.ContainsKey(eHero))
        {
            var save = Saves[eHero];
            save.SetNextTimeUnlock(UnbiasedTime.UtcNow + CooldownTime);
        }
        NextSessionNewHeroRecommend = EHero.None;
        Save();
    }

    public bool CanTriedHero()
    {
        bool isFirstDay = (UnbiasedTime.UtcNow.DayOfYear == DataManager.Save.General.DateFirstTime.DayOfYear);
        if (TryCount < maxTryCount.IntValue || (isFirstDay && TryCount < allHeroTriesSenerio.Count)) return true;
        return false;
    }

    public EHero GetRecommendHero()
    {
        foreach (var heroUnlocked in DataManager.Save.User.HeroSaves)
        {
            if (heroUnlocked.Value.IsUnlocked)
            {
                if (Saves.ContainsKey(heroUnlocked.Key))
                {
                    Saves.Remove(heroUnlocked.Key);
                    Save();
                }
            }
        }

        if (NextSessionNewHeroRecommend != EHero.None && !DataManager.Save.User.IsUnlockHero(NextSessionNewHeroRecommend))
        {
            return NextSessionNewHeroRecommend;
        }
        if (TryInAllGame < allHeroTriesSenerio.Count)
        {
            NextSessionNewHeroRecommend = allHeroTriesSenerio[TryInAllGame];
            while(TryInAllGame < allHeroTriesSenerio.Count)
            {
                if (DataManager.Save.User.IsUnlockHero(allHeroTriesSenerio[TryInAllGame])) 
                {
                    TryInAllGame++;
                }
                else
                {
                    break;
                }
            }
            if (TryInAllGame >= allHeroTriesSenerio.Count)
            {
                NextSessionNewHeroRecommend = EHero.None;
            }

            Save();
            return NextSessionNewHeroRecommend;
        }
        //return EHero.RocketHero;
        var allFreeHeros = new List<EHero>();
        foreach (var e in Saves)
        {
            if (e.Value.CanPick)
            {
                allFreeHeros.Add(e.Key);
            }
        }
        if (allFreeHeros.Count > 0)
        {
            NextSessionNewHeroRecommend = allFreeHeros.Random();
        }
        else
        {
            NextSessionNewHeroRecommend = EHero.None;
        }
        Save();
        return NextSessionNewHeroRecommend;
    }
}
