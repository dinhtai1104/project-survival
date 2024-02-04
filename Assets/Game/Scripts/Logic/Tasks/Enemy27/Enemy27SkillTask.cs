using com.mec;
using UnityEngine;
using System.Collections.Generic;
using Spine;
using Cysharp.Threading.Tasks;

public class Enemy27SkillTask : SkillTaskShotBullet
{
    public ValueConfigSearch angleZone;
    public ValueConfigSearch delayShot;
    public ValueConfigSearch minAngleBullet;
    public ValueConfigSearch numberBullet;

    public bool IsFinishByCompleteAnim = false;
    public override async UniTask Begin()
    {
        await base.Begin();
        angleZone = angleZone.SetId(Caster.gameObject.name);
        delayShot = delayShot.SetId(Caster.gameObject.name);
        minAngleBullet = minAngleBullet.SetId(Caster.gameObject.name);
        numberBullet = numberBullet.SetId(Caster.gameObject.name);
    }
    protected override void ReleaseBullet()
    {
        Timing.RunCoroutine(_Release(), gameObject);
    }
    protected override void OnCompleteTracking(TrackEntry trackEntry)
    {
        if (IsFinishByCompleteAnim)
        {
            if (trackEntry.Animation.Name == animationSkill)
            {
                IsCompleted = true;
            }
        }
    }
    public override void OnStop()
    {
        base.OnStop();
        Timing.KillCoroutines(gameObject);
    }
    private IEnumerator<float> _Release()
    {
        var centerAngle = GetAngleToTarget();
        var upAngle = centerAngle + angleZone.FloatValue;
        var downAngle = centerAngle - angleZone.FloatValue;
        float angle = centerAngle;
        float offset = 0;

        for (int i = 0; i < numberBullet.IntValue; i++)
        {
            ReleaseBullet(angle + offset);
            float angleRdCal = offset;
            for (int j = 0; j < 100 ; j++)
            {
                angleRdCal = Random.Range(minAngleBullet.FloatValue, minAngleBullet.FloatValue + 10);
                var up = angle + angleRdCal;
                var down = angle - angleRdCal;

                if (up > upAngle)
                {
                    continue;
                }
                if (down < downAngle)
                {
                    continue;
                }

                if (Random.value < 0.5f)
                {
                    offset = angleRdCal;
                }
                else
                {
                    offset = -angleRdCal;
                }
                break;

            }
            yield return Timing.WaitForSeconds(delayShot.FloatValue);
        }
        if (!IsFinishByCompleteAnim)
        {
            IsCompleted = true;
        }
    }
}