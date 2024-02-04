using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;


public class FreezeImpactStatusHandler : ImpactStatusHandler
{
    [SerializeField]
    private ValueConfigSearch StatusDuration, SlowRate;

    public override async UniTask Apply(ActorBase caster,ActorBase target,ImpactHandler handler )
    {
        var status = (await target.StatusEngine.AddStatus(caster, EStatus.Freeze, this));
        if (status != null)
        {
            status.Init(caster, target);
            status.SetDuration(StatusDuration.FloatValue);

             
            StatModifier modifier = new StatModifier(EStatMod.PercentAdd, SlowRate.FloatValue);
                var list = new List<AttributeStatModifier>()
            {
                new AttributeStatModifier { StatKey = StatKey.SpeedMove, Modifier = modifier }
            };
                ((FreezeStatus)status).AddFreezeStat(list);

        }
    }
}
