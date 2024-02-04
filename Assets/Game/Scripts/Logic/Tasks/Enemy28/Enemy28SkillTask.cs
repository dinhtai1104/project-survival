using com.mec;
using Cysharp.Threading.Tasks;
using Spine;
using System;
using System.Collections.Generic;

public class Enemy28SkillTask : SkillTaskShotBullet
{
    public ValueConfigSearch numberBullet;
    public ValueConfigSearch delayBullet;
    public ValueConfigSearch bulletSizeRect;

    public override async UniTask Begin()
    {
        await base.Begin();
        numberBullet = numberBullet.SetId(Caster.gameObject.name);
        delayBullet = delayBullet.SetId(Caster.gameObject.name);
        bulletSizeRect = bulletSizeRect.SetId(Caster.gameObject.name);
    }
    protected override void OnCompleteTracking(TrackEntry trackEntry)
    {
    }

    protected override void ReleaseBullet()
    {
        Timing.RunCoroutine(_Release());
    }

    private IEnumerator<float> _Release()
    {
        for (int i = 0; i < numberBullet.IntValue; i++)
        {
            var angle = GetAngleToTarget();
            var bullet = ReleaseBullet(angle);
            bullet.transform.localScale = bulletSizeRect.Vector2Value;
            yield return Timing.WaitForSeconds(delayBullet.FloatValue);
        }
        IsCompleted = true;
    }
}