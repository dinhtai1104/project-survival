using System;
using System.Collections.Generic;

[System.Serializable]
public class RarityEquipmentEntity
{
    public ERarity Rarity;
    public int MaxLevel;
    public float BonusStatMul;

    public StatModifier DmgBase;
    public StatModifier DmgGrown;

    public StatModifier HpBase;
    public StatModifier HpGrown;


    public RarityEquipmentEntity(DB_EquipmentRarity e)
    {
        Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
        MaxLevel = e.Get<int>("MaxLevel");
        BonusStatMul = e.Get<float>("BonusStatMul");

        DmgBase = new StatModifier(EStatMod.Flat, e.Get<float>("DmgBase"));
        DmgGrown = new StatModifier(EStatMod.Flat, e.Get<float>("DmgGrown"));
        HpBase = new StatModifier(EStatMod.Flat, e.Get<float>("HpBase"));
        HpGrown = new StatModifier(EStatMod.Flat, e.Get<float>("HpGrown"));
    }
}