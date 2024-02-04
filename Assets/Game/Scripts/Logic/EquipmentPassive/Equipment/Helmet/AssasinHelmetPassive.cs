
using Game.GameActor;
using System;
using UnityEngine;

public class AssasinHelmetPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epic_KillAmount;
    public ValueConfigSearch epic_HealHp;

    public ValueConfigSearch legendary_KillAmount;
    public ValueConfigSearch legendary_HealHp;

    private int killAmount = 0;
    private float healHp = 0;
    private int currentKill = 0;

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
    }

    private void OnKill(ActorBase attacker, ActorBase defender)
    {
        if (attacker != Caster) return;
        if (Rarity < ERarity.Epic) return;
        if (!defender.Tagger.HasTag(ETag.EnemyGround)) return;
        currentKill++;
        if (currentKill == killAmount)
        {
            Heal();
            currentKill = 0;
        }
    }

    private void Heal()
    {
        var value = Caster.HealthHandler.GetMaxHP() * healHp * Caster.Stats.GetValue(StatKey.HealMul);
        Debug.Log("[Passive Assasin Helmet] Hp added: " + value);

        Caster.Heal((int)value);
        Debug.Log("Heal Current: " + Caster.HealthHandler.GetHealth());
    }

    public override void Play()
    {
        base.Play();
        if (Rarity >= ERarity.Epic)
        {
            killAmount = epic_KillAmount.IntValue;
            healHp = epic_HealHp.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            killAmount = legendary_KillAmount.IntValue;
            healHp = legendary_HealHp.FloatValue;
        }
    }
}