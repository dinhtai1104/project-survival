using System;
using System.Collections.Generic;

[System.Serializable]
public class HeroTable : DataTable<EHero, HeroEntity>
{
    public override void GetDatabase()
    {
        DB_Hero.ForEachEntity(e =>
        {
            Get(e);
        });
    }

    public HeroEntity GetHero(EHero currentHero)
    {
        return Dictionary[currentHero];
    }

    private void Get(DB_Hero e)
    {
        var heroE = new HeroEntity(e);
        if (!Dictionary.ContainsKey(heroE.TypeHero))
        {
            Dictionary.Add(heroE.TypeHero, heroE);
        }
    }
}