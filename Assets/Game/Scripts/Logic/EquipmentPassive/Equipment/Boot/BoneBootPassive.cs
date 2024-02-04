using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using UnityEngine;

public class BoneBootPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicCooldownDroneDecrease;
    public ValueConfigSearch legendaryCooldownDroneDecrease;

    private float cooldownDroneDecrease;
    private bool IsInit = false;
    private StatModifier modifier;
    public override async void Play()
    {
        base.Play();
        if (IsInit) return;
        IsInit = true;
        
        if (Rarity >= ERarity.Epic)
        {
            cooldownDroneDecrease = epicCooldownDroneDecrease.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            cooldownDroneDecrease = legendaryCooldownDroneDecrease.FloatValue;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        // Apply Cooldown Drone
        modifier = new StatModifier(EStatMod.PercentAdd, -cooldownDroneDecrease);
        // Apply

        Debug.Log("[Passive Bone Boot] Appied Drone Decrease");

        var drone = ((BattleGameController)BattleGameController.Instance).Drone;
        if (drone == null) return;
        Debug.Log("[Passive Bone Boot] applied drone cool down from: " + Caster.name);
        drone.SkillEngine.AddModifierCooldownAllSkill(modifier);
    }
    public override void Remove()
    {
        base.Remove();
        var drone = ((BattleGameController)BattleGameController.Instance).Drone;
        if (drone == null) return;
        if (Caster == GameController.Instance.GetMainActor())
        {
            Debug.Log("[Passive Bone Boot] remove applied drone cool down from: " + Caster.name);
            drone.SkillEngine.RemoveModifierCooldown(0, modifier);
        }
    }
}