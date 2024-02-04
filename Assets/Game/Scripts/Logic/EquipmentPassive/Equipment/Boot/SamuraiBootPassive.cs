using System;
using UnityEngine;

public class SamuraiBootPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicHealHp;
    public ValueConfigSearch legendaryHealHp;
    bool isRemove = false;
    private float healHp;
    private void Start()
    {
        Messenger.AddListener<EPickBuffType>(EventKey.PickBuffDone, OnPickBuffDone);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener<EPickBuffType>(EventKey.PickBuffDone, OnPickBuffDone);
    }
    public override void Remove()
    {
        isRemove = true;
        base.Remove();
    }
    public override void Play()
    {
        base.Play();
        healHp = 0;
        if (Rarity >= ERarity.Epic)
        {
            healHp = epicHealHp.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            healHp = legendaryHealHp.FloatValue;
        }
    }
    private void OnPickBuffDone(EPickBuffType type)
    {
        if (isRemove) return;
        if (healHp != 0)
        {
            var value = Caster.HealthHandler.GetMaxHP() * healHp * Caster.Stats.GetValue(StatKey.HealMul);
            Debug.Log("[Passive Samurai Boot] Hp added after caster: " + value);

            Caster.Heal((int)value);
            Debug.Log("Heal Current: " + Caster.HealthHandler.GetHealth());
        }
    }
} 