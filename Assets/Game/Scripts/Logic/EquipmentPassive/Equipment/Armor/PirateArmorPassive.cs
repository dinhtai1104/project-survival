using System;
using UnityEngine;
public class PirateArmorPassive : BaseEquipmentPassive
{
    [SerializeField] private ParticleSystem shieldEff;
    public ValueConfigSearch epic_VirtualHp;
    public ValueConfigSearch epic_DurationVirtualHp;

    public ValueConfigSearch legendary_VirtualHp;
    public ValueConfigSearch legendary_DurationVirtualHp;

    private float virtualHp;
    private float durationVirtualHp;
    private float currentDuration = 0;
    private bool isShielding;

    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnStageStart(Callback cb)
    {
        if (Rarity < ERarity.Epic) return;
        Caster.HealthHandler.ResetArmor();
        Caster.HealthHandler.AddArmor((int)virtualHp);
        currentDuration = durationVirtualHp;
        isShielding = true;
        shieldEff.Play();
        Debug.Log("Current Armor: " + Caster.HealthHandler.GetArmor());
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!isShielding) return;
        if (Caster.HealthHandler.GetArmor() <= 0)
        {
            shieldEff.Stop();
            isShielding = false;
            Caster.HealthHandler.ResetArmor();
            return;
        }
        if (currentDuration >= 0)
        {
            currentDuration -= Time.deltaTime;
        }
        else
        {
            shieldEff.Stop();
            isShielding = false;
            Caster.HealthHandler.ResetArmor();
        }
    }

    public override void Play()
    {
        base.Play();
        float percent = 0;
        if (Rarity >= ERarity.Epic)
        {
            percent = epic_VirtualHp.FloatValue;
            durationVirtualHp = epic_DurationVirtualHp.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            percent = legendary_VirtualHp.FloatValue;
            durationVirtualHp = legendary_DurationVirtualHp.FloatValue;
        }
        virtualHp = Caster.Stats.GetValue(StatKey.Hp) * percent;
        isShielding = true;
    }
}