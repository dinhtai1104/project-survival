using BansheeGz.BGDatabase;
using System;
using System.Linq;

[System.Serializable]
public class HotSaleHeroTable : DataTable<int, HotSaleHeroEntity>
{
    public override void GetDatabase()
    {
        DB_HotSaleHero.ForEachEntity(e => Get(e));
    }

    private void Get(BGEntity e)
    {
        var entity = new HotSaleHeroEntity(e);
        Dictionary.Add(entity.Id, entity);
    }

    public HotSaleHeroEntity GetSaleHero(EHero hero)
    {
        return Dictionary.Values.ToList().Find(t => t.Hero == hero);
    }

}
