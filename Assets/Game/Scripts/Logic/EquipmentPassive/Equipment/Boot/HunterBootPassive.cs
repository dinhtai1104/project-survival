using Game.GameActor;
using System;
using UnityEngine;

public class HunterBootPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicDmgDecrease;
    public ValueConfigSearch legendaryDmgDecrease;

    public ShieldObject shieldFacing;
    private float dmgDecrease = 0;

    public override void Play()
    {
        base.Play();
        if (Rarity < ERarity.Epic) return;

        if (Rarity >= ERarity.Epic)
        {
            dmgDecrease = epicDmgDecrease.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            dmgDecrease = legendaryDmgDecrease.FloatValue;
        }
        shieldFacing.SetActor(Caster);
        shieldFacing.SetDmgDecrease(dmgDecrease);
    }
}