using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HotSaleHeroSaves : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<int, HotSaleHeroSave> Saves;
    public bool CanShowHeroSale = false;
    public EHero HeroShowSale = EHero.None;
    public HotSaleHeroSaves(string key) : base(key)
    {
        Saves = new Dictionary<int, HotSaleHeroSave>();
        var db = DataManager.Base.HotSaleHero;
        foreach (var entity in db.Dictionary)
        {
            if (!Saves.ContainsKey(entity.Key))
            {
                Saves.Add(entity.Key, new HotSaleHeroSave(entity.Key, entity.Value.Hero));
            }
        }
    }

    public override void Fix()
    {
        var db = DataManager.Base.HotSaleHero;
        foreach (var entity in db.Dictionary)
        {
            if (!Saves.ContainsKey(entity.Key))
            {
                Saves.Add(entity.Key, new HotSaleHeroSave(entity.Key, entity.Value.Hero));
            }
            Saves[entity.Key].Save = Save;
        }
    }

    public HotSaleHeroSave Get(EHero hero)
    {
        return Saves.ToList().Find(t => t.Value.Hero == hero).Value;
    }

    public void Active(int v)
    {
        var db = DataManager.Base.HotSaleHero;
        Saves[v].ActiveHotSale(db.Get(v).Time);
    }

    public void Active(EHero tryHero)
    {
        var db = DataManager.Base.HotSaleHero;

        Get(tryHero).ActiveHotSale(db.GetSaleHero(tryHero).Time);
    }

    public HotSaleHeroSave GetHeroNewest()
    {
        var all = Saves.Values.ToList().FindAll(t=>t.IsActived);
        if (all.Count == 0) return null;
        var sorted = all.OrderBy(t => t.TimeEnd).ToList();
        return sorted[0];
    }
}
