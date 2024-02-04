using System;
using System.Collections.Generic;

[System.Serializable]
public class HeroStarUpgradeTable : DataTable<EHero, List<HeroStarUpgradeEntity>>
{
    public override void GetDatabase()
    {
        DB_HeroStarUpgrade.ForEachEntity(e=>Get(e));
    }

    public List<HeroStarUpgradeEntity> GetUpgradesHero(EHero heroCurrent)
    {
        return Dictionary[heroCurrent];
    }

    private void Get(DB_HeroStarUpgrade e)
    {
        var heroUE = new HeroStarUpgradeEntity(e);
        if (!Dictionary.ContainsKey(heroUE.Hero))
        {
            Dictionary.Add(heroUE.Hero, new List<HeroStarUpgradeEntity>());
        }
        Dictionary[heroUE.Hero].Add(heroUE);
    }
}