using System;
using UnityEngine;

public class SamuraiHelmetPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epic_healHpPortal;
    public ValueConfigSearch legendary_healHpPortal;

    private float healHp = 0;

    private void OnEnable()
    {
        Messenger.AddListener<ERoomType>(EventKey.EnterRoom, OnEnterRoom);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ERoomType>(EventKey.EnterRoom, OnEnterRoom);
    }

    private void OnEnterRoom(ERoomType type)
    {
        if (type != ERoomType.Angel) return;
        Heal();
    }

    private void Heal()
    {
        var value = Caster.HealthHandler.GetMaxHP() * healHp * Caster.Stats.GetValue(StatKey.HealMul);
        Debug.Log("[Passive Samurai Helmet] Hp added: " + value);

        Caster.Heal((int)value);
        Debug.Log("Heal Current: " + Caster.HealthHandler.GetHealth());
    }

    public override void Play()
    {
        base.Play();
        if (Rarity >= ERarity.Epic)
        {
            healHp = epic_healHpPortal.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            healHp = legendary_healHpPortal.FloatValue;
        }
    }
} 