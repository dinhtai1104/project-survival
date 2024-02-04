
using Game.GameActor;
using System;
using UnityEngine;

public class AssasinArmorPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicDodge;
    public ValueConfigSearch legendaryDodge;
    private StatModifier dogeMod;
    public override void Play()
    {
        base.Play();
        dogeMod = new StatModifier(EStatMod.Flat, 0);
        Caster.Stats.AddModifier(StatKey.DodgeRate, dogeMod, this);

        Debug.Log("[Passive Assasin Armor] Appied DogeRate:" + dogeMod);

        if (itemEquipment.EquipmentRarity >= ERarity.Epic)
        {
            dogeMod.Value = epicDodge.FloatValue;
        }
        if (itemEquipment.EquipmentRarity >= ERarity.Legendary)
        {
            dogeMod.Value = legendaryDodge.FloatValue;
        }
    }
    private void OnDisable()
    {
        if (Caster != null)
        {
            Caster.Stats.RemoveModifier(StatKey.DodgeRate, dogeMod);
        }
    }
}