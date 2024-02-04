using Game.GameActor;
using System;
using UnityEngine;

public class FlameThrowerObject : CharacterDamageObject
{
    public Stat DurationFlameStatus;
    public Stat TickRateFlameStatus;
    public Stat IntervalDmg;
    public Stat DmgMulFlameStatus;
    private DamageSource damageSource;

    public ParticleSystem flameEff;
    private void OnEnable()
    {
        _hit.onTrigger += OnTriggerDmg;
    }
    public override void Play()
    {
        base.Play();

        _hit.SetIsFullTimeHit(true);
        _hit.SetIntervalTime(IntervalDmg.Value);
        flameEff.Play();
    }
    private void OnDisable()
    {
        _hit.onTrigger -= OnTriggerDmg;
    }
    private void OnTriggerDmg(Collider2D collider, ITarget target)
    {
        if (target == null) return;
        if ((UnityEngine.Object)target == Caster) return;
        ApplyFlameStatus(target);
    }

    private async void ApplyFlameStatus(ITarget target)
    {
        var flame = await ((ActorBase)target).StatusEngine.AddStatus(Caster, EStatus.Flame, this);
        if (flame == null) return;
        flame.Init(Caster, ((ActorBase)target));
        flame.SetDuration(DurationFlameStatus.Value);
        ((FlameStatus)flame).SetDmgMul(DmgMulFlameStatus.Value);
        ((FlameStatus)flame).SetCooldown(TickRateFlameStatus.Value);
    }
}