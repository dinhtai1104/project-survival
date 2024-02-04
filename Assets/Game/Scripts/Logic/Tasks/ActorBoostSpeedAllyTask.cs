using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Collections.Generic;
using UnityEngine;

public class ActorBoostSpeedAllyTask : SkillTask
{
    public ValueConfigSearch Rate;
    public ValueConfigSearch Time;

    public ECharacterType targetType;

    float boostTime = 0;

    List<ActorBase> targets = new List<ActorBase>();
    public override async UniTask Begin()
    {
        boostTime = Time.IntValue;

        this.targets.Clear();
        ActorBase[] targets = FindObjectsOfType<ActorBase>();
        foreach (var target in targets)
        {
            if (targetType.Contains(target.GetCharacterType()))
            {
                this.targets.Add(target);
            }
        }

        SetSpeed(Rate.FloatValue + Caster.Stats.GetValue(StatKey.BoostFireRateAdditional,0));

        Messenger.Broadcast<bool>(EventKey.SpeedBoost,true);

        await base.Begin();
    }
    public override async UniTask End()
    {
        await base.End();
        foreach (var target in targets)
        {
            target.Stats.RemoveModifiersFromSource(this);
        }
        Messenger.Broadcast<bool>(EventKey.SpeedBoost, false);
    }
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;
        if (boostTime > 0)
        {
            boostTime -= UnityEngine.Time.deltaTime;
        }
        else
        {
            IsCompleted = true;
       


        }

    }

    void SetSpeed(float rate)
    {
        foreach (var target in targets)
        {
            target.Stats.AddModifier(StatKey.FireRate, new StatModifier(EStatMod.PercentAdd, rate),this);
            GameObjectSpawner.Instance.Get("VFX_Player_SpeedDrone", res =>
            {
                res.GetComponent<Game.Effect.EffectAbstract>().Active(target.GetPosition()).SetParent(target.GetTransform());
            });
        }
    }
}