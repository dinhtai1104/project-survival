using com.mec;
using Cysharp.Threading.Tasks;
using Game.Pool;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class Enemy7022Attack : RangeShotTask
{
    private int currentIndex = 0;
    public ValueConfigSearch angleOffset;
    private int count = 0;
    private float[] angle = new float[3];
    public override async UniTask Begin()
    {
        count = 0;
        angle[0] = -angleOffset.SetId(Caster.gameObject.name).FloatValue;
        angle[1] = 0;
        angle[2] = angleOffset.SetId(Caster.gameObject.name).FloatValue;
        await base.Begin();
    }

    protected override void OnCompleteTracking(TrackEntry trackEntry)
    {
    }
    protected override IEnumerator<float> _Shot(Transform pos)
    {
        yield return Timing.WaitUntilDone(base._Shot(pos));
        IsCompleted = true;
    }

    //protected override void OnEventTracking(TrackEntry trackEntry, Event e)
    //{
    //    if (trackEntry.Animation.Name == animationSkill)
    //    {
    //        if (e.Data.Name == "attack_tracking")
    //        {
    //            if (count >= bulletNumber.IntValue)
    //            {
    //                return;
    //            }
    //            if (!string.IsNullOrEmpty(VFX_Name))
    //            {
    //                GameObjectSpawner.Instance.Get(VFX_Name, (t) =>
    //                {
    //                    t.GetComponent<Game.Effect.EffectAbstract>().Active(pos.position);
    //                });
    //            }


    //            eventShoot?.Invoke();
    //            ReleaseBullet(GetAngleToTarget());
    //            count++;

    //            Caster.AnimationHandler.SetAnimation(1, animationSkill, false);
    //        }
    //    }
    //}

    protected override BulletSimpleDamageObject ReleaseBullet(Transform pos, float angle)
    {
        angle += this.angle[currentIndex];
        currentIndex++;
        currentIndex %= 3;
        return base.ReleaseBullet(pos, angle);
    }
}