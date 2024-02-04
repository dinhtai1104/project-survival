using System;
using UnityEngine;

public class PirateBootPassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicImmuneDuration;
    public ValueConfigSearch legendaryImmuneDuration;

    private float duration = 0;
    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private async void OnStageStart(Callback cb)
    {
        if (Rarity < ERarity.Epic) return;
        var status = await Caster.StatusEngine.AddStatus(Caster, EStatus.Immune, this);
        if (status == null) return;
        status.Init(Caster, Caster);
        status.SetDuration(duration);
    }

    public override void Play()
    {
        base.Play();
        if (Rarity >= ERarity.Epic)
        {
            duration = epicImmuneDuration.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            duration = legendaryImmuneDuration.FloatValue;
        }
    }
}