using System;

public class HeroUpgradeStarNotifyCondition : NotifyCondition<EHero>
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
        try
        {
            var save = DataManager.Save.User.GetHero(hero);
            if (save == null || !save.IsUnlocked)
            {
                return false;
            }
            var currentStar = save.Star;
            var db = DataManager.Base.Hero.Get(hero);
            var upgrade = db.StarUpgrades;
            if (currentStar > upgrade.Count)
            {
                return false;
            }
            var nextLevel = upgrade[currentStar];
            var res = DataManager.Save.Resources;
            if (res.HasResource(nextLevel.HeroFragment))
            {
                return true;
            }
        }
        catch (Exception ex)
        {

        }

        return false;
    }
}