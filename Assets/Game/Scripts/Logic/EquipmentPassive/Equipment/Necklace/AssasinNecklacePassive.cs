
using Game.GameActor;
using System;
using UnityEngine;

public class AssasinNecklacePassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicHeadshot;
    public ValueConfigSearch legendaryHeadshot;

    private StatModifier headShotRate;

    public override void Remove()
    {
        base.Remove();
        Caster.Stats.RemoveModifier(StatKey.HeadshotRate, headShotRate);
    }
    public override void Play()
    {
        base.Play();
        headShotRate = new StatModifier(EStatMod.Flat, 0);
        if (itemEquipment.EquipmentRarity >= ERarity.Epic)
        {
            headShotRate.Value = epicHeadshot.FloatValue;
        }
        if (itemEquipment.EquipmentRarity >= ERarity.Legendary)
        {
            headShotRate.Value = legendaryHeadshot.FloatValue;
        }
        Caster.Stats.AddModifier(StatKey.HeadshotRate, headShotRate, this);
        Debug.Log("[Passive Assasin Necklace] Appied HeadshotRate:" + headShotRate);
    }
}