using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HeroLevelUpgradeTable : DataTable<EHero, List<HeroLevelUpgradeEntity>>
{
    public override void GetDatabase()
    {
        DB_HeroLevelUpgrade.ForEachEntity(e => Get(e));
    }

    public List<HeroLevelUpgradeEntity> GetUpgradesHero(EHero heroCurrent)
    {
        return Dictionary[heroCurrent];
    }

    private void Get(DB_HeroLevelUpgrade e)
    {
        var heroUE = new HeroLevelUpgradeEntity(e);
        if (!Dictionary.ContainsKey(heroUE.Hero))
        {
            Dictionary.Add(heroUE.Hero, new List<HeroLevelUpgradeEntity>());
        }
        Dictionary[heroUE.Hero].Add(heroUE);
    }
}