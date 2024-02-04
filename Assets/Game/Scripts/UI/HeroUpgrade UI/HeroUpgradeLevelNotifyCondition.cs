using System;

public class HeroUpgradeLevelNotifyCondition : NotifyCondition<EHero>
{
    public override bool Validate()
    {
        foreach (EHero hero in Enum.GetValues(typeof(EHero)))
        {
            if (Validate(hero)) { return true; }
        }
        return false;
    }
    public override bool Validate(EHero hero)
    {
        var save = DataManager.Save.User.GetHero(hero);
        if (save == null || !save.IsUnlocked)
        {
            return false;
        }
        var currentLevel = save.Level;
        var db = DataManager.Base.Hero.Get(hero);
        var upgrade = db.LevelUpgrades;
        if (currentLevel >= upgrade.Count)
        {
            return false;
        }
        var nextLevel = upgrade[currentLevel];

        var res = DataManager.Save.Resources;
        if (res.HasResource(nextLevel.Fragment) && res.HasResource(nextLevel.Cost))
        {
            return true;
        }

        return false;
    }
}