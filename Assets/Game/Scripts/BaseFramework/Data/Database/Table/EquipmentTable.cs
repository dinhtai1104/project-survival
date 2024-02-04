using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EquipmentTable : DataTable<string, EquipmentEntity>
{
    public override void GetDatabase()
    {
        // Get Database Here
        DB_Equipment.ForEachEntity(e => Get(e));
    }

    private void Get(DB_Equipment e)
    {
        var entity = new EquipmentEntity(e);
        Dictionary.Add(entity.Id, entity);
    }

    public EquipmentEntity GetEntity(int Id)
    {
        return Dictionary.Values.ToList().Find(t => t.IdNum == Id);
    }

    public EquipmentEntity GetEntity(string id)
    {
        return Dictionary[id];
    }

    public EquipableItem GetItem(string id)
    {
        var eq = Dictionary[id].Clone();
        var eqStatBase = new StatAffix(eq.StatKey, new StatModifier(eq.StatMod, eq.StatBase));
        eqStatBase.DescriptionKey = eq.DescriptionKey;
        var eqItem = new EquipableItem(eq);

        eqItem.SetBaseStat(eqStatBase);
        if (eq.Type == EEquipment.MainWeapon)
        {
            var mw = DataManager.Base.Weapon.Dictionary[eq.Id];
            eqItem.AddBaseAffix(new StatAffix(StatKey.FireRate, new StatModifier(EStatMod.Flat, mw.FireRate)));
            eqItem.AddBaseAffix(new StatAffix(StatKey.SpeedBullet, new StatModifier(EStatMod.Flat, mw.BulletVelocity)));
        }

        eqItem.EquipmentType = eq.Type;

        // add affix
        AddAffix(ERarity.UnCommon, eqItem);
        AddAffix(ERarity.Common, eqItem);
        AddAffix(ERarity.Rare, eqItem);
        AddAffix(ERarity.Epic, eqItem);
        AddAffix(ERarity.Legendary, eqItem);
        AddAffix(ERarity.Ultimate, eqItem);
        

        return eqItem;
    }
    private void AddAffix(ERarity rarity, EquipableItem item)
    {
        var entity = Dictionary[item.Id];
        if (entity.Affixes.ContainsKey(rarity))
        {
            item.AddBaseAffix(rarity, entity.Affixes[rarity]);
        }
        else
        {
            item.AddBaseAffix(rarity, NullAffix.Null);
        }
        //// add affix
        //var affixTable = DataManager.Base.AffixEquipment;
        //if (affixTable.HasAffixEquipment(item.Id))
        //{
        //    var affixFilter = affixTable.AffixGroupFilterEquipment[item.Id];
        //    var affixes = affixFilter.GetFilter(rarity);
        //    foreach (var affix in affixes)
        //    {
        //        var affixCvt = new StatAffix(affix.Attribute);
        //        affixCvt.DescriptionKey = affix.DescriptionKey;
        //        item.AddBaseAffix(rarity, affixCvt);
        //    }
        //}
    }

    public bool HasEquipment(string id)
    {
        return Dictionary.ContainsKey(id);
    }
}
