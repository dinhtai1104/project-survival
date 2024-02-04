using Game.GameActor;
using System;

public class KunaiPassive : WeaponPassive
{
    public ValueConfigSearch epic_BleedingDuration;
    public ValueConfigSearch epic_BleedingDmgMul;
    public ValueConfigSearch epic_BleedingTickrate;

    public ValueConfigSearch legendary_BleedingDmgMul;

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
    }

    private async void OnAttackEvent(ActorBase attacker, ActorBase defender)
    {
        if (attacker != Caster) return;
        if (Rarity < ERarity.Epic) return;
        var flame = await defender.StatusEngine.AddStatus(attacker, EStatus.Bleeding, this);
        if (flame != null)
        {
            flame.Init(attacker, defender);
            flame.SetDuration(epic_BleedingDuration.FloatValue);

            ((BleedStatus)flame).SetCooldown(epic_BleedingTickrate.FloatValue);
            float dmgMul = 1f;
            if (Rarity >= ERarity.Epic)
            {
                dmgMul = epic_BleedingDmgMul.FloatValue;
            }
            if (Rarity >= ERarity.Legendary)
            {
                dmgMul = legendary_BleedingDmgMul.FloatValue;
            }

            ((BleedStatus)flame).SetDmgMul(dmgMul);
        }
    }
}
