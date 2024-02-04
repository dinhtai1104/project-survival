using Game.GameActor;
using Game.GameActor.Buff;
using System;
using UnityEngine;

public class FocusFireBuff : AbstractBuff
{
    private Stat focusFireStat;
    private float stepIncreaseDmg = 0;
    private float maxIncreaseDmg = 0.5f;
    private float currentIncreaseDmg = 0;

    private ActorBase lastActorDefender;
    public override void Initialize(ActorBase Caster, BuffEntity entity, int stageStart)
    {
        base.Initialize(Caster, entity, stageStart);
        focusFireStat = new Stat();
    }
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);        
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
    {
        if (attacker != Caster) return;
        if (lastActorDefender == null)
        {
            lastActorDefender = defender;
            focusFireStat.ClearModifiers();
            focusFireStat.BaseValue = 0;
            return;
        }
       
        if (lastActorDefender == defender)
        {
            focusFireStat.AddModifier(new StatModifier(EStatMod.Flat, stepIncreaseDmg));
            damageSource.Value *= (1 + focusFireStat.Value);
        }
        else
        {
            lastActorDefender = defender;
            focusFireStat.ClearModifiers();
            focusFireStat.BaseValue = 0;
        }
    }

    public override void Play()
    {
        stepIncreaseDmg = GetValue(StatKey.MinValue);
        maxIncreaseDmg = GetValue(StatKey.MaxValue);

        focusFireStat.SetConstraintMin(0);
        focusFireStat.SetConstraintMax(maxIncreaseDmg);
        focusFireStat.BaseValue = focusFireStat.Value;
        focusFireStat.RecalculateValue();
    }
}
