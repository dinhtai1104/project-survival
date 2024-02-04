using System;

public class HeroCanUnlockNotifyCondition : NotifyCondition<EHero>
{
    public override bool Validate(EHero dependency)
    {
        try
        {
            var save = DataManager.Save.User.GetHero(dependency);
            if (save == null || save.IsUnlocked) return false;

            var db = DataManager.Base.Hero.Get(dependency);
            if (DataManager.Save.Resources.HasResource(db.HeroResource, db.NumberFragmentUnlock))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
        return false;
    }
    public override bool Validate()
    {
        foreach (EHero hero in Enum.GetValues(typeof(EHero)))
        {
            if (Validate(hero)) { return true; }
        }
        return false;
    }
}
