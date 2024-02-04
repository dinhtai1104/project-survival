using Game.GameActor;
using System;
using UnityEngine;

public class RailGunWeaponVisual : Weapon
{
    [SerializeField] private RailGunEnergy railGunEnergy;
    public ValueConfigSearch maxStack;

    public override void SetUp(Character character, WeaponBase weapon)
    {
        base.SetUp(character, weapon);
        railGunEnergy.SetMaxStack(maxStack.IntValue);
    }
    public override void Trigger()
    {
        base.Trigger();
        railGunEnergy.Stack();
    }
}