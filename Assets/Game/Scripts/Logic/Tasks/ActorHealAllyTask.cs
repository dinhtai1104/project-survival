using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealAllyTask : SkillTask
{
    public ValueConfigSearch HealRate;
    public ValueConfigSearch HealTime;

    public ECharacterType targetType;

    float healTime = 0;
    float rate = 0;
    float time = 0;

    List<ActorBase> targets=new List<ActorBase>();
    public override async UniTask Begin()
    {
        healTime = HealTime.IntValue;
        rate = HealRate.FloatValue+Caster.Stats.GetValue(StatKey.HealRateAdditional,0);

        this.targets.Clear();
        ActorBase[] targets = FindObjectsOfType<ActorBase>();
        foreach(var target in targets)
        {
            if (targetType.Contains(target.GetCharacterType()))
            {
                this.targets.Add(target);
            }
        }
        await base.Begin();
    }
    public override async UniTask End()
    {
        await base.End();
        Logger.Log("END "+gameObject.name);
        foreach (var target in targets)
        {
            target.Heal((int)healTime*rate * target.HealthHandler.GetMaxHP());
        }
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (healTime > 0)
        {
            healTime -= Time.deltaTime;
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 1;
                Heal();
            }
        }
        else
        {
            IsCompleted = true;
        }

    }

    void Heal()
    {
        foreach(var target in targets)
        {
            target.Heal(rate*target.HealthHandler.GetMaxHP());
        }
    }
}
