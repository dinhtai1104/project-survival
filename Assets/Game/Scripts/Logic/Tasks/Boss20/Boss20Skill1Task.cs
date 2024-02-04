using Cysharp.Threading.Tasks;
using Spine;
using System;
using UnityEngine;

public class Boss20Skill1Task : SkillTask
{
    [SerializeField] private ValueConfigSearch dmgSkill;
    [SerializeField] private ValueConfigSearch radiusSkill;

    [SerializeField] private DamageExplode damageEffPrefab;
    [SerializeField] private string _animationDapDat;
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.onEventTracking += OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking += OnCompleteTracking;
        Caster.AnimationHandler.SetAnimation(_animationDapDat, false);
    }

    private void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animationDapDat)
        {
            IsCompleted = true;
        }
    }

    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= OnCompleteTracking;
        return base.End();
    }

    private void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animationDapDat)
        {
            if (e.Data.Name == "attack_tracking")
            {
                var eff = PoolManager.Instance.Spawn(damageEffPrefab);
                eff.transform.position = Caster.GetPosition();
                eff.Init(Caster);
                eff.SetSize(radiusSkill.FloatValue);
                eff.SetDmg(Caster.Stats.GetValue(StatKey.Dmg) * dmgSkill.FloatValue);

                eff.Explode();
            }
        }
    }
}