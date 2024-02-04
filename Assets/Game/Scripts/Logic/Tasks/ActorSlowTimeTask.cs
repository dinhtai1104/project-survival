using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorSlowTimeTask : SkillTask
{
    public ValueConfigSearch SlowRate;
    public ValueConfigSearch TimeSlow;

    public ECharacterType targetType;

    public AnimationCurve moveCurve;

    float time = 99;
    float slowRate = 0;


    public override async UniTask Begin()
    {

        slowRate = SlowRate.FloatValue;
        time = TimeSlow.FloatValue+Caster.Stats.GetValue(StatKey.TimeSlowAddictional,0);
        SetTime(slowRate);
        Messenger.Broadcast<bool>(EventKey.FreezeTime,true);
        await base.Begin();

    }
    public override async UniTask End()
    {
        SetTime(1);
        Messenger.Broadcast<bool>(EventKey.FreezeTime,false);
        await base.End();

    }

    async UniTask SetTime(float scale)
    {
        float time = 0;
        float duration = moveCurve.keys[moveCurve.length - 1].time;
        float from = GameTime.Controller.TIME_SCALE;
        while (time < duration)
        {
            GameTime.Controller.TIME_SCALE = from+moveCurve.Evaluate(time) * (scale-from);

            time += Time.deltaTime;
            await UniTask.Yield();
        }
        GameTime.Controller.TIME_SCALE =scale;

    }
    private void OnPreFire(ActorBase actor, BulletBase bullet, List<ModifierSource> modifier)
    {
        //if bullet shot from Caster or not the target we want => skip  
        if (actor == Caster || !targetType.Contains(actor.GetCharacterType())) return;
        Logger.Log($"PREFIRE: {bullet.gameObject.name} {modifier[0].Value} {slowRate}");
        modifier[0].AddModifier(new StatModifier(EStatMod.PercentAdd, slowRate));
    }

    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        
        if (time > 0)
        {
            time -= Time.unscaledDeltaTime;
        }
        else
        {
            IsCompleted = true;
        }

    }
}