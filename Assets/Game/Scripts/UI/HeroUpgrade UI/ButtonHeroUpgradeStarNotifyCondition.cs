using System;

public class ButtonHeroUpgradeStarNotifyCondition : NotifyCondition
{
    private EHero currentSelectHero;
    private void OnEnable()
    {
        currentSelectHero = DataManager.Save.User.Hero;
        Messenger.AddListener<EHero>(EventKey.SelectHeroInventory, OnSelectHero);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<EHero>(EventKey.SelectHeroInventory, OnSelectHero);
    }

    private void OnSelectHero(EHero type)
    {
        currentSelectHero = type;
        this.notifyMono?.Validate();
    }

    public override bool Validate()
    {
        return Validate(currentSelectHero);
    }
    public bool Validate(EHero hero)
    {
        if (hero == EHero.None) return false;
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
            if (currentStar >= upgrade.Count)
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
