using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;


public class KnockbackImpactStatusHandler : ImpactStatusHandler
{
    [SerializeField]
    private ValueConfigSearch Power;

    [SerializeField]
    private Vector3 direction,directionMultiplier=Vector3.one;
    public override async UniTask Apply(ActorBase caster,ActorBase target,ImpactHandler handler )
    {
        if (target!=null &&target.GetCharacterType() == ECharacterType.Enemy)
        {
            Vector2 force = ((target.GetMidTransform().position - caster.GetMidTransform().position).normalized + direction) * Power.FloatValue;
            force.x *= directionMultiplier.x;
            force.y *= directionMultiplier.y;
            target.StatusEngine.AddStatus(caster, EStatus.KnockBack, this).ContinueWith(status =>
            {
                KnockBackStatus knockBack = status as KnockBackStatus;
                knockBack.AddForce(force);
                knockBack.SetDuration(0.7f);
            }).Forget();
        }
    }
}
