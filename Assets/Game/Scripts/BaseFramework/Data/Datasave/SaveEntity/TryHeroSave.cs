using Foundation.Game.Time;
using System;

[System.Serializable]
public class TryHeroSave
{
    public EHero Hero;
    public DateTime NextTimeUnlock;
    public System.Action Save;
    public bool CanPick => (NextTimeUnlock - UnbiasedTime.UtcNow).TotalSeconds <= 0 && !DataManager.Save.User.IsUnlockHero(Hero);

    public TryHeroSave(EHero hero, DateTime NextTimeUnlock)
    {
        Hero = hero;
        this.NextTimeUnlock = NextTimeUnlock;
    }

    public void SetNextTimeUnlock(DateTime nextTime, bool save = false)
    {
        NextTimeUnlock = nextTime;
    }
}