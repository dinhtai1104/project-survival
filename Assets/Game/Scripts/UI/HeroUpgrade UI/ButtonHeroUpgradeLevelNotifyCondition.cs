using System;

public class ButtonHeroUpgradeLevelNotifyCondition : NotifyCondition
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
